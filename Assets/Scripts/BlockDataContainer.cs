using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BlockDataContainer : ScriptableObject
{
    [SerializeField] public List<BlockData> blockDatas;

    public BlockData GetBlockDataById(int id) {
        if (blockDatas.Count < 0 || id >= blockDatas.Count) {
            Debug.LogError("Index out of list limits");
            return null;
        }

        return blockDatas[id];
    }
}
