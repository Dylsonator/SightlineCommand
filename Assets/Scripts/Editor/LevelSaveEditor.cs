using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EditorFunction))]
public class LevelSaveEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Save Level", GUILayout.Height(40))) {
            EditorFunction script = (EditorFunction)target;
            GridGenerator gen = script.genUse;

            List<TileData> tileList = new List<TileData>();
            foreach (var newData in script.tileData) {
                tileList.Add(newData);
            }

            AssetDatabase.Refresh();
            LevelSave levelSave = (LevelSave)ScriptableObject.CreateInstance("LevelSave");
            levelSave.tiles = tileList;
            levelSave.width = gen.width;
            levelSave.height = gen.height;
            levelSave.scale = gen.scale;
            levelSave.gapScale = gen.gapScale;

            AssetDatabase.CreateAsset(levelSave, $"Assets/Scriptable Objects/Level Saves/{script.nameInput.text}.asset");
        }
    }
}