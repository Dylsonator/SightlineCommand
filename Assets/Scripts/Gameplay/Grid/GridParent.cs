using UnityEngine;
using System.Collections.Generic;

public class GridParent : MonoBehaviour {
    public Tile[,] tiles;

    public void SetGridSize(int x, int z) {
        tiles = new Tile[x,z];
    }

    public void ClearTiles() {
        foreach (var tile in tiles) { 
            Destroy(tile.gameObject);
        }
    }
}
