using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum BLOCK_SIDE { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK }

public class Block
{
    public Mesh mesh = null;

    public Block(int blockId, Vector3 offset, Chunk chunk) {
        if (blockId != -1) {
            BlockDataContainer blockDataContainer = ContainersManager.Instance.GetBlockDataContainer();

            BlockData blockData = blockDataContainer.GetBlockDataById(blockId);

            Vector2[] topuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.TOP));
            Vector2[] bottomuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BOTTOM));

            Vector2[] rightuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.RIGHT));
            Vector2[] leftuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.LEFT));

            Vector2[] frontuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.FRONT));
            Vector2[] backuvs = TextureAtlasHelper.GetAtlasPositionBasedOnIndex(blockData.GetAtlasIndexBySide(BLOCK_SIDE.BACK));

            List<Quad> quads = new List<Quad>();

            List<Mesh> meshes = new List<Mesh>();
            if (!chunk.HasSolidBlock(offset + Vector3.up)) {
                quads.Add(new Quad(BLOCK_SIDE.TOP, offset, topuvs));
            }
                // meshes.Add(q.GenerateQuad(BLOCK_SIDE.TOP, offset, topuvs));
            if (!chunk.HasSolidBlock(offset + Vector3.down)) {
                quads.Add(new Quad(BLOCK_SIDE.BOTTOM, offset, bottomuvs));
            }
            if (!chunk.HasSolidBlock(offset + Vector3.left))   {
                quads.Add(new Quad(BLOCK_SIDE.RIGHT, offset, rightuvs));
            }          
            if (!chunk.HasSolidBlock(offset + Vector3.right)) {
                quads.Add(new Quad(BLOCK_SIDE.LEFT, offset, leftuvs));
            }    
            if (!chunk.HasSolidBlock(offset + Vector3.forward)) {
                quads.Add(new Quad(BLOCK_SIDE.FRONT, offset, frontuvs));
            }
            if (!chunk.HasSolidBlock(offset + Vector3.back)) {
                quads.Add(new Quad(BLOCK_SIDE.BACK, offset, backuvs));
            }

            if (quads.Count == 0) {
                return;
            }

            Mesh[] meshArray = new Mesh[quads.Count];
            int i = 0;
            foreach(Quad q in quads) {
                meshArray[i] = q.mesh;
                i+=1;
            }

            mesh = MeshUtils.MergeMeshes(meshArray);
        }
    }
}
