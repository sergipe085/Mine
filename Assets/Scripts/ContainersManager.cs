using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainersManager : MonoBehaviour
{
    [SerializeField] private BlockDataContainer blockDataContainer;
    public BlockDataContainer GetBlockDataContainer() => blockDataContainer;

    public static ContainersManager Instance = null;

    private void Awake() {
        Instance = this;
    }
}
