using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum BLOCK_SIDE { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK }

public class Block
{
    public Mesh mesh;

    public Block(int blockId, Vector3 offset) {
        Quad q = new Quad();

        BlockDataContainer blockDataContainer = ContainersManager.Instance.GetBlockDataContainer();

        BlockData blockData = blockDataContainer.GetBlockDataById(blockId);

        Vector2[] topuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.TOP));
        Vector2[] bottomuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BOTTOM));

        Vector2[] rightuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.RIGHT));
        Vector2[] leftuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.LEFT));

        Vector2[] frontuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.FRONT));
        Vector2[] backuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BACK));

        Mesh[] meshes = new Mesh[6];
        meshes[0] = q.GenerateQuad(BLOCK_SIDE.TOP, offset, topuvs);
        meshes[1] = q.GenerateQuad(BLOCK_SIDE.BOTTOM, offset, bottomuvs);
        meshes[2] = q.GenerateQuad(BLOCK_SIDE.RIGHT, offset, rightuvs);
        meshes[3] = q.GenerateQuad(BLOCK_SIDE.LEFT, offset, leftuvs);
        meshes[4] = q.GenerateQuad(BLOCK_SIDE.FRONT, offset, frontuvs);
        meshes[5] = q.GenerateQuad(BLOCK_SIDE.BACK, offset, backuvs);

        mesh = MeshUtils.MergeMeshes(meshes);
    }
}
