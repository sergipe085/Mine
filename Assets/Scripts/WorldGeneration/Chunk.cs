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

    public Vector3Int size = Vector3Int.one;

    public Block[,,] blocks;
    public int[] chunkData;

    private void Start() {
        GenerateMeshAsync();
    }

    private void BuildChunk() {
        int blockCount = size.x * size.y * size.z;
        chunkData = new int[blockCount];

        for (int i = 0; i < blockCount; i++) {
           
            float n = UnityEngine.Random.Range(0f, 1f);
            if (n <= 0.5f) {
                chunkData[i] = -1;
            }
            else {
                chunkData[i] = 1;
            }
        }
    }

    private void GenerateChunk() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        blocks = new Block[size.x, size.y, size.z];

        BuildChunk();

        List<Mesh> meshes = new List<Mesh>();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    blocks[x, y, z] = new Block(chunkData[ ConvertIndex(x, y, z)], new Vector3(x, y, z), this);
                    if (blocks[x, y, z].mesh != null) {
                        meshes.Add(blocks[x, y, z].mesh);
                    }
                }
            }
        }

        Mesh mergedMeshes = MeshUtils.MergeMeshes(meshes.ToArray());

        mf.mesh = mergedMeshes;
    }

    private void GenerateMeshAsync() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        blocks = new Block[size.x, size.y, size.z];

        BuildChunk();

        List<Mesh> inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = size.x * size.y * size.z;
        int m = 0;
        var job = new ProcessMeshDataJob();
        job.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        job.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    blocks[x, y, z] = new Block(chunkData[ ConvertIndex(x, y, z)], new Vector3(x, y, z), this);
                    if (blocks[x, y, z].mesh != null) {
                        inputMeshes.Add(blocks[x, y, z].mesh);
                        int vCount = blocks[x, y, z].mesh.vertexCount;
                        int iCount = (int)blocks[x, y, z].mesh.GetIndexCount(0);
                        job.vertexStart[m] = vertexStart;
                        job.triStart[m] = triStart;
                        vertexStart += vCount;
                        triStart += iCount;
                        m+=1;
                    }
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

        var handle = job.Schedule(inputMeshes.Count, 4);
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

    public int ConvertIndex(int x, int y, int z) {
        return z * size.x * size.y + y * size.x + x;
    }

    public bool HasSolidBlock(Vector3 pos) {
        int index = ConvertIndex((int)pos.x, (int)pos.y, (int)pos.z);

        if (pos.x < 0 || pos.x >= size.x) {
            return false;
        }

        if (pos.y < 0 || pos.y >= size.y) {
            return false;
        }

        if (pos.z < 0 || pos.z >= size.z) {
            return false;
        }

        return chunkData[index] != -1;
        // return _chunkData[(int)pos.x, (int)pos.y, (int)pos.z] != -1;
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
