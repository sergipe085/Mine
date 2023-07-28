using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lr = null;
    public float heightScale = 2f;
    public float scale = 0.5f;
    public int octaves = 1;
    public float height = 1.0f;

    private void Start() {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 100;
        Graph();
    }

    public float FBM(float x, float z) {
        float total = 0.0f;
        float frequency = 1f;
        for (int i = 0; i < octaves; i++) {
            total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2f;
        }
        return total;
    }

    private void Graph() {
        int z = 11;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int x = 0; x < lr.positionCount; x++) {
            float y = FBM(x, z) + height;
            positions[x] = new Vector3(x, y, z);
        }
        lr.SetPositions(positions);
    }

    private void OnValidate() {
        Graph();
    }
}
