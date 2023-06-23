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
    public int blockId = 0;
    public int blockTopId = 0;
    public int blockBottomId = 0;

    private void Start() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        Quad q = new Quad();

        BlockDataContainer blockDataContainer = ContainersManager.Instance.GetBlockDataContainer();

        BlockData blockData = blockDataContainer.GetBlockDataById(blockId);

        Vector2[] topuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.TOP));
        Vector2[] bottomuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BOTTOM));

        Vector2[] rightuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.RIGHT));
        Vector2[] leftuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.LEFT));

        Vector2[] frontuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.FRONT));
        Vector2[] backuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BACK));
        // var meshes = Enum.GetValues(typeof(BLOCK_SIDE)).Cast<BLOCK_SIDE>().Select(side => q.GenerateQuad(side, Vector3.zero, uvs));

        Mesh[] meshes = new Mesh[6];
        meshes[0] = q.GenerateQuad(BLOCK_SIDE.TOP, Vector3.zero, topuvs);
        meshes[1] = q.GenerateQuad(BLOCK_SIDE.BOTTOM, Vector3.zero, bottomuvs);
        meshes[2] = q.GenerateQuad(BLOCK_SIDE.RIGHT, Vector3.zero, rightuvs);
        meshes[3] = q.GenerateQuad(BLOCK_SIDE.LEFT, Vector3.zero, leftuvs);
        meshes[4] = q.GenerateQuad(BLOCK_SIDE.FRONT, Vector3.zero, frontuvs);
        meshes[5] = q.GenerateQuad(BLOCK_SIDE.BACK, Vector3.zero, backuvs);

        Mesh merged = MeshUtils.MergeMeshes(meshes);

        mf.mesh = merged;
        mf.name = "Cube_0_0_0";
        mf.mesh.name = "Cube_0_0_0";
        Debug.Log($"Vertices: {merged.vertices.Length} Normals: {merged.normals.Length} UVs: {merged.uv.Length} Tris: {merged.triangles.Length}");
    }
}
