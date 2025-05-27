using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UnifiedCursor : MonoBehaviour
{
    public enum InputMode { Mouse, HandTracking }
    public InputMode currentInputMode = InputMode.Mouse;

    [Header("Common")]
    public PlayerTeam CurrentTeam;

    [Header("Mouse Mode Settings")]
    private LayerMask rayLayers;

    [Header("Hand Mode Settings")]
    public GameObject Fingertip;
    public float handRayDistance = 500f;

    [Header("UI & State")]
    public TextMeshProUGUI modeDisplay;

    // Selection state
    private Unit activeUnit = null;
    private Unit EnemyUnit;
    private TextMeshPro activeValuesText;

    // Modes
    public enum UnitMode { None, Attack, Move, Build, End }
    public UnitMode currentMode = UnitMode.None;

    // Internal
    private bool handSelectionCheck = false;

    private void Start()
    {
        UpdateModeDisplay();
    }
    private void Awake()
    {
        // Replace with your actual layers by name
        rayLayers = LayerMask.GetMask("Default", "Unit", "Building", "Tile");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("[BUILD DEBUG] Mouse click received");
            System.IO.File.AppendAllText(Application.persistentDataPath + "/debug_clicks.txt", $"Mouse click at {Time.time}\n");
        }
        // Toggle input mode on T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentInputMode = currentInputMode == InputMode.Mouse ? InputMode.HandTracking : InputMode.Mouse;
            ClearAll();
            UpdateModeDisplay();
            Debug.Log("Switched input mode to: " + currentInputMode);
        }

        switch (currentInputMode)
        {
            case InputMode.Mouse:
                HandleMouseInput();
                break;

            case InputMode.HandTracking:
                HandleHandInput();
                break;
        }
    }

    // Public method for UI to change mode and update display
    public void UpdateModeDisplay(int modeIndex)
    {        
        SetBehaviour(modeIndex);
        modeDisplay.text = "Input Mode:" + currentInputMode + "\n Current Mode: " + currentMode;
      
    }

    // Private method to update the UI text only (called internally)
    private void UpdateModeDisplay()
    {
        if (modeDisplay != null)
            modeDisplay.text = "Input Mode:" + currentInputMode + "\n Current Mode: " + currentMode;
    }

    // ----------- MOUSE MODE --------------

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightClickBehaviour();
        }

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit cursorHit;

        if (Physics.Raycast(cursorRay, out cursorHit, Mathf.Infinity, rayLayers))
        {
            if (Input.GetMouseButtonDown(0) && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            {
                GameManager.SelectionChanged?.Invoke();

                switch (cursorHit.collider.tag)
                {
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

    // ----------- HAND MODE -------------

    private void HandleHandInput()
    {
        if (!handSelectionCheck)
            return;

        Vector3 originFinger = Fingertip.transform.position;
        RaycastHit fingerHit;

        if (Physics.Raycast(originFinger, Fingertip.transform.right, out fingerHit, handRayDistance))
        {
            switch (fingerHit.collider.tag)
            {
                case "Unit":
                    UnitClickBehaviour(fingerHit.collider.GetComponentInParent<Unit>());
                    break;

                case "Tile":
                    TileClickBehaviour(fingerHit.collider.GetComponentInParent<Tile>());
                    break;

                case "Building":
                    BuildingClickBehaviour(fingerHit.collider.GetComponent<Building>());
                    break;
            }
        }
    }

    public void HandSelectionStart()
    {
        StartCoroutine(WaitToConfirm());
    }

    public void HandSelectionEnd()
    {
        StartCoroutine(WaitToDeselect());
    }

    private IEnumerator WaitToConfirm()
    {
        handSelectionCheck = true;
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator WaitToDeselect()
    {
        handSelectionCheck = false;
        yield return new WaitForSeconds(2.5f);
    }

    // ----------- COMMON SELECTION BEHAVIOURS -----------

    public void UnitClickBehaviour(Unit unit)
    {
        if (unit == null)
            return;

        if (activeUnit == null && currentMode != UnitMode.None && unit.Health != 0 && !unit.inAction)
        {
            activeUnit = unit;
            activeValuesText = unit.valuesText;

            if (activeUnit.team == CurrentTeam)
            {
                switch (currentMode)
                {
                    case UnitMode.Attack:
                        if (unit.CurrentAttacks > 0)
                        {
                            activeValuesText.text = unit.CurrentAttacks.ToString();
                            unit.CurrentMoveableCol = unit.moveableCol[0];
                            unit.MarkAdjacentTiles(unit.currentTile, unit.AttackRange, true);
                        }
                        else
                        {
                            ClearAll();
                        }
                        break;

                    case UnitMode.Move:
                        activeValuesText.text = unit.CurrentMove.ToString();
                        if (activeUnit.CurrentMove > 0)
                        {
                            unit.BeginMove();
                        }
                        else
                        {
                            ClearAll();
                        }
                        break;

                    case UnitMode.Build:
                        if (activeUnit.currentTile.buildingHere == null)
                        {
                            activeUnit.ShowBuildMenu();
                        }
                        ClearAll();
                        break;
                }
            }
            else
            {
                ClearAll();
            }
        }
        else if (currentMode == UnitMode.Attack)
        {
            TileClickBehaviour(unit.currentTile);
        }
    }

    public void TileClickBehaviour(Tile tile)
    {
        if (tile == null)
            return;

        bool acted = false;

        if (activeUnit != null)
        {
            switch (currentMode)
            {
                case UnitMode.Attack:
                    if (tile.unitHere && activeUnit.enemiesInSight.Contains(tile.unitHere))
                    {
                        activeUnit.AttackUnit(tile);
                        acted = true;
                    }
                    else if (tile.buildingHere && activeUnit.buildingsInSight.Contains(tile.buildingHere))
                    {
                        activeUnit.AttackBuilding(tile);
                        acted = true;
                    }
                    break;

                case UnitMode.Move:
                    if (tile.unitHere)
                    {
                        acted = true;
                        break;
                    }
                    if (tile.terrainType.walkable || activeUnit.isFlying)
                    {
                        activeUnit.EndMove(tile);
                        acted = true;
                    }
                    break;
            }

            if (acted)
            {
                ClearAll();
            }
        }
    }

    public void BuildingClickBehaviour(Building building)
    {
        if (building == null)
            return;

        if (activeUnit != null && activeUnit.team != building.team)
        {
            TileClickBehaviour(building.tile);
            return;
        }

        if (activeUnit == null && building.team == CurrentTeam)
        {
            GameManager.Instance.gameUI.buildingPanel.SetActive(true);
            GameManager.Instance.gameUI.buildingPanel.GetComponent<BuildingPanel>().SetBuilding(building);
        }
        else
        {
            TileClickBehaviour(building.tile);
        }
    }

    protected void RightClickBehaviour()
    {
        ClearAll();
    }

    public void ClearAll()
    {
        if (activeUnit != null)
        {
            activeUnit.EndTargeting();
            activeUnit = null;

            if (activeValuesText != null)
            {
                activeValuesText.text = "";
                activeValuesText = null;
            }
        }
    }

    // Set mode (Attack, Move, Build, etc.) by index from UI buttons
    public void SetBehaviour(int modeIndex)
    {
        UnitMode selectedMode = (UnitMode)modeIndex;
        if (currentMode == selectedMode)
        {
            currentMode = UnitMode.None;
        }
        else
        {
            currentMode = selectedMode;
        }
        ClearAll();
        UpdateModeDisplay();
    }

    public void EndTurn()
    {
        ClearAll();
        GameManager.Instance.EndTurn(CurrentTeam);
        CurrentTeam = CurrentTeam == PlayerTeam.HUMAN ? PlayerTeam.ALIEN : PlayerTeam.HUMAN;
        GameManager.Instance.NewTurn(CurrentTeam);
    }
}
