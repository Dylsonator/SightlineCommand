using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathTile {
    public Tile tile;
    public int minCost = 99;
    public List<Tile> path;

    public PathTile(Tile tile, int minCost, List<Tile> path) {
        this.tile = tile;
        this.minCost = minCost;
        this.path = path;
    }
}

public class Tile : MonoBehaviour {
    public TileTerrain terrainType;
    public List<Tile> adjacentTiles = new List<Tile>();

    public Unit unitHere;
    public bool IsSelected;
    public Building buildingHere;
    public GameObject decoration;

    public Action<Unit> UnitMovedHere;

    private Renderer thisRenderer;
    public bool lerpingColour;
    public bool highlighted;
    public float lerpTime;

    public Vector2 coords;

    [HideInInspector]
    public int decoIndex;
    [HideInInspector]
    public Vector3 decoRotation;

    private void Awake() {
        thisRenderer = transform.Find("TileMesh").GetComponent<Renderer>();
    }

    public void OnDestroy() {
        if (decoration != null) {
            Destroy(decoration);
        }
    }

    public void ClearTerrain() {
        if (decoration != null) {
            Destroy(decoration);
            decoration = null;
        }
        if (buildingHere != null) {
            Destroy(buildingHere.gameObject);
            buildingHere = null;
        }
        if (unitHere != null) {
            Destroy(unitHere.gameObject);
            unitHere = null;
        }
    }

    public void SetTerrain() {
        if (decoration != null) {
            Destroy(decoration);
            decoration = null;
        }
        thisRenderer.material = terrainType.material;
        bool hasDecoration = false;
        if (terrainType.decorations.Count > 0 && buildingHere == null) {
            if (UnityEngine.Random.value <= terrainType.decorationFrequency) {
                decoIndex = UnityEngine.Random.Range(0, terrainType.decorations.Count);
                decoration = Instantiate(terrainType.decorations[decoIndex], transform);

                int alignment = UnityEngine.Random.Range(0, 6);
                decoRotation = decoration.transform.rotation.eulerAngles;
                decoRotation.y = alignment * 60;
                decoration.transform.rotation = Quaternion.Euler(decoRotation);
                hasDecoration = true;
            }
        }
        if (!hasDecoration) {
            decoIndex = -1;
        }
    }

    public void LoadTile(TileData loadTile) {
        ClearTerrain();
        terrainType = loadTile.terrainType;
        thisRenderer.material = terrainType.material;
        if (loadTile.decoration != null) {
            decoration = Instantiate(loadTile.decoration, transform);
            decoration.transform.rotation = Quaternion.Euler(loadTile.decorationRotation);
        }
        if (loadTile.buildingHere != null) {
            CreateBuilding(loadTile.buildingHere.GetComponent<Building>());
        }
        if (loadTile.unitHere != null) {
            GameObject unitObj = Instantiate(loadTile.unitHere);
            unitObj.GetComponent<Unit>().UnitSpawn(this);
        }
    }

    public void CreateBuilding(Building building) {
        if (buildingHere != null || !terrainType.walkable) {
            return;
        }

        if (decoration != null) {
            Destroy(decoration.gameObject);
            decoration = null;
        }

        buildingHere = Instantiate(building.gameObject, transform).GetComponent<Building>();
        buildingHere.tile = this;
        buildingHere.SpawnBehaviour();
        GameManager.Instance.players[building.team].buildings.Add(buildingHere);
    }

    public void DisplayColour(Color color) {
        if (lerpingColour) {
            return;
        }
        lerpingColour = true;
        StartCoroutine(LerpColour(color));
    }

    public void ResetMaterial()
    {
        lerpingColour = false;
        lerpTime = 0;
        thisRenderer.material = terrainType.material;
    }

    private IEnumerator LerpColour(Color color) {
        //will add a proper easing here later
        while (lerpingColour) {
            float perc = lerpTime < 0.5f ? lerpTime * 1.75f : 1 - (lerpTime - 0.5f) * 1.75f;
            Color newColour = Color.Lerp(terrainType.material.color, color, perc);

            thisRenderer.material.color = newColour;

            lerpTime += Time.deltaTime;
            if (lerpTime >= 1) {
                lerpTime = 0;
            }
            yield return null;
        }
    }

    public List<PathTile> GetWalkableGroup(int maxMovement, bool flying = false) {
        List<PathTile> tilePaths = new List<PathTile>(); 
        tilePaths = WeightedSearch(new PathTile(this, 0, new List<Tile>()), 0, maxMovement, tilePaths, flying);
        return tilePaths;
    }

    public List<PathTile> WeightedSearch(PathTile tileToCheck, int currentCost, int maxMovement, List<PathTile> pathList, bool flying) {

        foreach (Tile adjacentTile in tileToCheck.tile.adjacentTiles){
            if (!adjacentTile.terrainType.walkable && !flying) { //checks if the unit is a flying one to allow it to ignore terrain affordability and inaccessable tiles. (Dylan)
                continue;
            }
            int nextTravelSpeed = flying ? 1 : adjacentTile.terrainType.travelSpeed;
            int newCost = currentCost + nextTravelSpeed;

            PathTile adjacentPathTile = new PathTile(this, 0, new List<Tile>());

            bool inList = false;
            foreach (PathTile path in pathList) { 
                if (adjacentTile == path.tile) {
                    adjacentPathTile = path;
                    inList = true;
                    if (path.minCost > newCost) {
                        path.minCost = newCost;
                        List<Tile> newPath = new List<Tile>(tileToCheck.path);
                        newPath.Add(adjacentTile);
                        path.path = newPath;
                    }
                    break;
                }
            }
            if (!inList && newCost <= maxMovement) {
                List<Tile> newPath = new List<Tile>(tileToCheck.path);
                newPath.Add(adjacentTile);
                adjacentPathTile = new PathTile(adjacentTile, currentCost, newPath);
                pathList.Add(adjacentPathTile);
            }
            if (newCost < maxMovement) {
                pathList = WeightedSearch(adjacentPathTile, newCost, maxMovement, pathList, flying);
            }
        }
        return pathList;
    }

    public List<Tile> GetAdjacentGroup(int maxLoops) {
        List<Tile> tileList = new List<Tile>();
        tileList = AdjacentSearch(this, 0, maxLoops, tileList);
        return tileList;
    }

    public List<Tile> AdjacentSearch(Tile tileToCheck, int loopNo, int maxLoops, List<Tile> tileList) {
        loopNo++;
        foreach (Tile adjacentTile in tileToCheck.adjacentTiles) {
            if (!tileList.Contains(adjacentTile)) { 
                tileList.Add(adjacentTile);
            }
            if (loopNo < maxLoops) {
                tileList = AdjacentSearch(adjacentTile, loopNo, maxLoops, tileList);
            }
        }
        return tileList;
    }

    public Tile FindEmptyTile() {
        if (!unitHere) {
            return this;
        } 
        foreach (Tile tile in adjacentTiles) {
            if (!tile.unitHere && tile.terrainType.walkable) {
                return tile;
            }
        }
        return this; //currently returns self as a fallback. might add additional check layer later
    }
}
