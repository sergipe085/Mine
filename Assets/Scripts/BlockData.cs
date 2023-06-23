using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BlockData : ScriptableObject
{
    public string blockName = "AIR";

    public List<BlockAtlasSideMapper> atlasMapper;

    public Vector2 GetAtlasIndexBySide(BLOCK_SIDE side) {
        BlockAtlasSideMapper blockAtlasSideMapper = atlasMapper.Find(x => x.side == side);

        if (blockAtlasSideMapper == null) {
            blockAtlasSideMapper = atlasMapper[0];
        }

        return blockAtlasSideMapper.atlasIndex;
    }
}

[System.Serializable]
public class BlockAtlasSideMapper {
    public BLOCK_SIDE side;
    public Vector2 atlasIndex;
}
