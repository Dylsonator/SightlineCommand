using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileEditorPanel : MonoBehaviour {
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject noContent;

    [SerializeField] private TMP_Dropdown terrain;
    [SerializeField] private TMP_Dropdown building;
    [SerializeField] private TMP_Dropdown unit;
    [SerializeField] private TMP_Dropdown decoration;
    [SerializeField] private Slider decoRotation;

    private Tile currentTile;
    private bool loadingData;

    private void Start() {
        List<string> terrainNames = new List<string>();
        foreach (var terrainType in EditorFunction.Instance.terrains) {
            terrainNames.Add(terrainType.terrainName);
        }
        terrain.AddOptions(terrainNames);

        List<string> buildingNames = new List<string>();
        foreach (var building in EditorFunction.Instance.buildings) {
            buildingNames.Add(building.name);
        }
        building.AddOptions(buildingNames);

        List<string> unitNames = new List<string>();
        foreach (var unit in EditorFunction.Instance.units) {
            unitNames.Add(unit.name);
        }
        unit.AddOptions(unitNames);
    }

    public void ShowTile(Tile tile, TileData tileData) {
        loadingData = true;
        content.SetActive(true);
        noContent.SetActive(false);

        currentTile = tile;

        SetDecorationNames();

        terrain.value = EditorFunction.Instance.terrains.IndexOf(tile.terrainType);
        building.value = EditorFunction.Instance.buildings.IndexOf(tileData.buildingHere) + 1;
        unit.value = EditorFunction.Instance.units.IndexOf(tileData.unitHere) + 1;
        loadingData = false;
    }

    private void SetDecorationNames() {
        decoration.ClearOptions();

        List<string> decoNames = new List<string>();
        decoNames.Add("None");
        foreach (var decorations in currentTile.terrainType.decorations) {
            decoNames.Add(decorations.name);
        }
        decoration.AddOptions(decoNames);
        decoration.value = currentTile.decoIndex + 1;
        if (decoration.value != 0) {
            decoRotation.gameObject.SetActive(true);
            decoRotation.value = currentTile.decoRotation.y;
        } else {
            decoRotation.gameObject.SetActive(false);
        }
    }

    public void HideTile() {
        content.SetActive(false);
        noContent.SetActive(true);
    }

    public void RotationUpdate(float value) {
        if (loadingData) { 
            return;
        }
        float incValue = Mathf.Round(value / 60) * 60;
        decoRotation.value = incValue;
        currentTile.decoRotation = new Vector3(0, incValue, 0);
        currentTile.decoration.transform.rotation = Quaternion.Euler(currentTile.decoRotation);
        EditorFunction.Instance.UpdateTileData(currentTile);
    }

    public void TerrainUpdate(int index) {
        if (loadingData) {
            return;
        }
        var terrainType = EditorFunction.Instance.terrains[index];
        currentTile.terrainType = terrainType;
        currentTile.SetTerrain();
        EditorFunction.Instance.UpdateTileData(currentTile);
        SetDecorationNames();
    }

    public void DecorationUpdate(int index) {
        if (loadingData) {
            return;
        }
        if (currentTile.decoration != null) { 
            Destroy(currentTile.decoration);
            currentTile.decoration = null;
        }
        currentTile.decoIndex = index - 1;
        if (currentTile.decoIndex != -1) {
            building.value = 0;
            decoRotation.gameObject.SetActive(true);
            GameObject newDecoration = Instantiate(currentTile.terrainType.decorations[currentTile.decoIndex], currentTile.transform);
            newDecoration.transform.rotation = Quaternion.Euler(currentTile.decoRotation);
            currentTile.decoration = newDecoration;
        } else {
            decoRotation.gameObject.SetActive(false);
        }
        EditorFunction.Instance.UpdateTileData(currentTile);
    }

    public void BuildingUpdate(int index) {
        if (loadingData) {
            return;
        }
        if (currentTile.buildingHere != null) {
            Destroy(currentTile.buildingHere.gameObject);
            currentTile.buildingHere = null;
        }
        
        GameObject returnBuilding = null;
        int buildingIndex = index - 1;
        if (buildingIndex != -1) {
            decoration.value = 0;
            unit.value = 0;
            returnBuilding = EditorFunction.Instance.buildings[buildingIndex];
            currentTile.CreateBuilding(returnBuilding.GetComponent<Building>());
        }
        EditorFunction.Instance.UpdateBuilding(returnBuilding, currentTile);
    }

    public void UnitUpdate(int index) {
        if (loadingData) {
            return;
        }
        if (currentTile.unitHere != null) {
            Destroy(currentTile.unitHere.gameObject);
            currentTile.unitHere = null;
        }
        GameObject returnUnit = null;
        int unitIndex = index - 1;
        if (unitIndex != -1) {
            building.value = 0;
            returnUnit = EditorFunction.Instance.units[unitIndex];
            GameObject newUnit = Instantiate(returnUnit, currentTile.transform);
            newUnit.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            currentTile.unitHere = newUnit.GetComponent<Unit>();
        }
        EditorFunction.Instance.UpdateUnit(returnUnit, currentTile);
    }
}
