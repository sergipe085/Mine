using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    public Material atlas;

    public Vector3Int size = Vector3Int.one * 2;

    public Block[,,] blocks;

    private void Start() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        blocks = new Block[size.x, size.y, size.z];

        Mesh[] inputMeshes = new Mesh[blocks.Length];
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = blocks.Length;
        int m = 0;
        var job = new ProcessMeshDataJob();
        job.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        job.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        
        int i = 0;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    blocks[x, y, z] = new Block(0, new Vector3(x, y, z));
                    inputMeshes[i] = blocks[x, y, z].mesh;
                    int vCount = inputMeshes[i].vertexCount;
                    int iCount = (int)inputMeshes[i].GetIndexCount(0);
                    job.vertexStart[m] = vertexStart;
                    job.triStart[m] = triStart;
                    vertexStart += vCount;
                    triStart += iCount;
                    i+=1;
                    m+=1;
                }
            }
        }

        job.meshDataArray = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        job.outputMesh = outputMeshData[0];
        job.outputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
        job.outputMesh.SetVertexBufferParams(vertexStart, 
            new VertexAttributeDescriptor(VertexAttribute.Position), 
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream:1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream:2)
        );

        var handle = job.Schedule(meshCount, 4);
        var newMesh = new Mesh();
        newMesh.name = "CHUNK";
        var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;

        handle.Complete();

        job.outputMesh.subMeshCount = 1;
        job.outputMesh.SetSubMesh(0, sm);
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });
        job.meshDataArray.Dispose();
        job.vertexStart.Dispose();
        job.triStart.Dispose();
        newMesh.RecalculateBounds();

        mf.mesh = newMesh;
    }

    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor {
        [ReadOnly] public Mesh.MeshDataArray meshDataArray;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triStart;

        public void Execute(int index) {
            Mesh.MeshData data = meshDataArray[index];
            int vCount = data.vertexCount;
            int vStart = vertexStart[index];

            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());

            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());

            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>(0);
            var outputNormals = outputMesh.GetVertexData<Vector3>(1);
            var outputUvs = outputMesh.GetVertexData<Vector3>(2);

            for (int i = 0; i < vCount; i++) {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUvs[i + vStart] = uvs[i];
            }

            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            int tStart = triStart[index];
            int tCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();
            if (data.indexFormat == IndexFormat.UInt16) {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < tCount; i++) {
                int idx = tris[i];
                outputTris[i+tStart] = vStart + idx;
            }
                
            }
            else {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < tCount; i++) {
                    int idx = tris[i];
                    outputTris[i+tStart] = vStart + idx;
                }
            }
        }
    }
}
