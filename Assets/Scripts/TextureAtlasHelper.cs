using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TextureAtlasHelper
{
    public static Vector2[] GetAtlasPositionBasedOnIndex(Vector2 index) {
        Vector2[] uvs = new Vector2[4];

        int atlasWidth = 256;
        int atlasHeight = 256;
        int blocksHoritonzal = 16;
        int blocksVertical = 16;
        
        Vector2 unitSize = new Vector2(1f / (atlasWidth / blocksHoritonzal), 1f / (atlasHeight / blocksVertical));
    
        int uvIndex = 0;
        for (int y = 0; y <= 1; y++) {
            for (int x = 0; x <= 1; x++) {
                float _x = unitSize.x * (index.x + x);
                float _y = unitSize.y * (index.y + y);
                uvs[uvIndex] = new Vector2(_x, _y);
                uvIndex += 1;

                Debug.Log(new Vector2(_x, _y));
            }
        }

        return uvs;
    }
}

// #if UNITY_EDITOR
// [CustomEditor(typeof(TextureAtlasHelper))]
// public class TextureAtlasHelperEditor : Editor {
//     TextureAtlasHelper textureAtlasHelper;

//     public void OnEnable()
//     {
//         textureAtlasHelper = (TextureAtlasHelper)target;
//     }

//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();

//         if (GUILayout.Button("EXTRACT")) {

//             Vector2 unitSize = new Vector2(1f / (textureAtlasHelper.atlasWidth / textureAtlasHelper.blocksHoritonzal), 1f / (textureAtlasHelper.atlasHeight / textureAtlasHelper.blocksVertical));
            
//             textureAtlasHelper.uvs.Clear();
//             textureAtlasHelper.uvsString = "";
//             for (int y = 0; y <= 1; y++) {
//                 for (int x = 0; x <= 1; x++) {
//                     float _x = unitSize.x * (textureAtlasHelper.blockXIndex + x);
//                     float _y = unitSize.y * (textureAtlasHelper.blockYIndex + y);
//                     textureAtlasHelper.uvs.Add(new Vector2(_x, _y));
//                     textureAtlasHelper.uvsString += $"new Vector2({_x}f, {_y}f);\n";
//                 }
//             }
//         }
//     }
// }
// #endif