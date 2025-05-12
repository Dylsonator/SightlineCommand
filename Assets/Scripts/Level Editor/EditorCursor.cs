using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EditorMode {
    Edit,
    Paint
}

public class EditorCursor : CursorControls {
    public TileTerrain terrainBrush;
    public EditorMode mode;
    private bool painting;
    private List<Tile> paintedTiles;

    private void Awake() {
        active = true;
    }

    protected override void Update() {
        switch (mode) {
            case EditorMode.Edit:
                base.Update();
                break;
            case EditorMode.Paint:
                if (Input.GetMouseButton(0)) {
                    if (!painting) {
                        painting = true;
                        paintedTiles = new List<Tile>();
                    }
                    Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit cursorHit;
                    if (Physics.Raycast(cursorRay, out cursorHit)) {
                        if (cursorHit.collider.tag == "Tile") {
                            TilePaint(cursorHit.collider.GetComponentInParent<Tile>());
                        }
                    }
                } else {
                    painting = false;
                }
                break;
        }
    }

    private void TilePaint(Tile tile) {
        if (paintedTiles.Contains(tile)) { 
            return;
        }
        paintedTiles.Add(tile);
        tile.terrainType = terrainBrush;
        tile.SetTerrain();
        EditorFunction.Instance.UpdateTileData(tile);
    }

    protected override void TileClickBehaviour(Tile tile) {

        EditorFunction.Instance.SelectTile(tile);
    }
}
