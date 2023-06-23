using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum BLOCK_SIDE { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK }

public class Block : MonoBehaviour
{
    public Material atlas;

    private void Start() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        Quad q = new Quad();

        var meshes = Enum.GetValues(typeof(BLOCK_SIDE)).Cast<BLOCK_SIDE>().Select(side => q.GenerateQuad(side,Vector3.zero));

        Mesh merged = MeshUtils.MergeMeshes(meshes.ToArray());

        mf.mesh = merged;
        mf.name = "Cube_0_0_0";
        mf.mesh.name = "Cube_0_0_0";
        Debug.Log($"Vertices: {merged.vertices.Length} Normals: {merged.normals.Length} UVs: {merged.uv.Length} Tris: {merged.triangles.Length}");
    }
}
