using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorFunction : MonoBehaviour {
    public static EditorFunction Instance;

    public GridGenerator gridGeneratorPrefab;

    public GridGenerator genUse;

    public TMP_Dropdown terrainSelect;

    public EditorCursor editorCursor;

    public TMP_InputField nameInput;

    public List<GameObject> buildings;

    public List<GameObject> units;

    public List<TileTerrain> terrains;

    public TileEditorPanel tileEditorPanel;

    public TileData[,] tileData;

    private void Awake() {
        if (Instance == null) { 
            Instance = this;
        } else {
            Destroy(this);
        }
        genUse.inEditor = true;
        GameManager.Instance.editorStart = true;
        terrains = gridGeneratorPrefab.terrainTypes;
    }

    private void Start() {
        List<string> terrainNames = new List<string>();
        foreach (var terrain in terrains) {
            terrainNames.Add(terrain.terrainName);
        }
        terrainSelect.AddOptions(terrainNames);
        editorCursor.terrainBrush = terrains[0];
        genUse.GenerateGrid();
        tileData = new TileData[genUse.width, genUse.height];
        foreach (var tile in GameManager.Instance.tiles) {
            var newTileData = new TileData(tile);
            int x = (int)tile.coords[0];
            int z = (int)tile.coords[1];
            tileData[x,z] = newTileData;
        }
    }

    public void UpdateTileData(Tile tile) {
        tileData[(int)tile.coords[0], (int)tile.coords[1]].UpdateTerrain(tile);
    }

    public void UpdateBuilding(GameObject building, Tile tile) {
        tileData[(int)tile.coords[0], (int)tile.coords[1]].buildingHere = building;
    }

    public void UpdateUnit(GameObject unit, Tile tile) {
        tileData[(int)tile.coords[0], (int)tile.coords[1]].unitHere = unit;
    }

    public void EditorModeChanged(int index) {
        terrainSelect.gameObject.SetActive(index == 1);
        if (index == 1) {
            tileEditorPanel.HideTile();
        }
        editorCursor.mode = (EditorMode)index;
    }

    public void SelectTile(Tile tile) {
        tileEditorPanel.ShowTile(tile, tileData[(int)tile.coords[0], (int)tile.coords[1]]);
    }

    public void TerrainValueChanged(int index) {
        editorCursor.terrainBrush = terrains[index];
    }
}
