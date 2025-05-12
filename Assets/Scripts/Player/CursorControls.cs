using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CursorControls : MonoBehaviour { 
    //By default, the cursor can hit layers 'Default' (1), 'Building' (64), and 'Unit' (128).
    [SerializeField]
    private LayerMask rayLayers = 193;

    public bool active;

    //More efficient ways of doing this can be made later
    protected virtual void Update() {
        if (!active) {
            return;
        }
        if (Input.GetMouseButtonDown(1)) {
            RightClickBehaviour();
        }
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit cursorHit;
        if (Physics.Raycast(cursorRay, out cursorHit, Mathf.Infinity, rayLayers)) {
            switch (cursorHit.collider.tag) {
                case "Tile":
                    //TileHoverBehaviour(cursorHit.collider.GetComponentInParent<Tile>());
                    break;
            }
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                GameManager.SelectionChanged?.Invoke();
                switch (cursorHit.collider.tag) {
                    case "Unit":
                        UnitClickBehaviour(cursorHit.collider.GetComponentInParent<Unit>());
                        break;
                    case "Tile":
                        TileClickBehaviour(cursorHit.collider.GetComponentInParent<Tile>());
                        break;
                    case "Building":
                        BuildingClickBehaviour(cursorHit.collider.GetComponent<Building>());
                        break;
                }
            }
        }
    }

    public virtual void UnitClickBehaviour(Unit unit) { }

    protected virtual void TileClickBehaviour(Tile tile) { }

    protected virtual void BuildingClickBehaviour(Building building) { }

    protected virtual void TileHoverBehaviour(Tile tile) { }

    protected virtual void RightClickBehaviour() { }
}
