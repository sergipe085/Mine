using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BLOCK_SIDE { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK }

public class Block : MonoBehaviour
{
    [SerializeField] private BLOCK_SIDE side = BLOCK_SIDE.BOTTOM;
    [SerializeField] private Vector3 offset = Vector3.zero;

    private void Start() {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();

        Quad q = new Quad();

        mf.mesh = q.GenerateQuad(side, offset);
    }
}
