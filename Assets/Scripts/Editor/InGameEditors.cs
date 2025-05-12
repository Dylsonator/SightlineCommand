using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Unit), true)]
public class UnitEditor : Editor {
    public override void OnInspectorGUI() { 
        base.OnInspectorGUI();

        if (GUILayout.Button("Select This Unit")) {
            GameManager.Instance.gameCursor.UnitClickBehaviour(target as Unit);
        }
    }
}

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Select This Tile")) {
            GameManager.Instance.gameCursor.TileClickBehaviour(target as Tile);
        }
    }
}

[CustomEditor(typeof(Building), true)]
public class BuildingEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Select This Building")) {
            GameManager.Instance.gameCursor.BuildingClickBehaviour(target as Building);
        }
    }
}
