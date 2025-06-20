using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class UnifiedCursor : MonoBehaviour
{
    [SerializeField] private FingerLine fingerLine;

    public enum InputMode { Mouse, HandTracking }
    public static UnifiedCursor.InputMode GlobalInputMode = InputMode.Mouse;

    [Header("Common")]
    public PlayerTeam CurrentTeam;    
    public TutorialCursor TutorialCursor = null;

    [SerializeField, Header("Mouse Mode Settings")]
    private LayerMask rayLayers;

    [Header("Hand Mode Settings")]
    public GameObject Fingertip;
    public float handRayDistance = Mathf.Infinity;

    [Header("UI & State")]
    public TextMeshProUGUI modeDisplay;
    private List<GameObject> Hands = new List<GameObject>();

    // Selection state
    private Unit activeUnit = null;
    private Unit EnemyUnit;
    private TextMeshPro activeValuesText;

    // Modes
    public enum UnitMode { None, Attack, Move, Build, End }
    public UnitMode currentMode = UnitMode.None;

    // Internal
    private bool handSelectionCheck = false;

    private bool canSwapMode = true;

    public bool active;

    private void Start()
    {
        UpdateModeDisplay();
    }
        private void Awake()
    {
        rayLayers = LayerMask.GetMask("Default", "Unit", "Building");
    }

    private void Update()
    {
        // Toggle input mode on T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            GlobalInputMode = GlobalInputMode == InputMode.Mouse ? InputMode.HandTracking : InputMode.Mouse;
            ClearAll();
            UpdateModeDisplay();
            Debug.Log("Switched input mode to: " + GlobalInputMode);
            if (fingerLine != null)
            {
                fingerLine.SetActive(GlobalInputMode == InputMode.HandTracking);
            }
            switch (GlobalInputMode)
            {
                case InputMode.Mouse:
                    for (int i = 0; i < Hands.Count; i++)
                    {
                        Hands[i].GetComponent<FingerLine>().enabled = false;
                    }
                    break;

                case InputMode.HandTracking:
                    for (int i = 0; i < Hands.Count; i++)
                    {
                        Hands[i].GetComponent<FingerLine>().enabled = true;
                    }
                    break;
            }
        }

        if (!active)
        {
            return;
        }
        switch (GlobalInputMode)
        {
            case InputMode.Mouse:
                HandleMouseInput();                                
                break;

            case InputMode.HandTracking:
                //HandleHandInput();                         
                break;
        }
    }

    // Public method for UI to change mode and update display
    public void UpdateModeDisplay(int modeIndex)
    {        
        SetBehaviour(modeIndex);
        modeDisplay.text = "Input Mode:" + GlobalInputMode + "\n Current Mode: " + currentMode;      
    }

    // Private method to update the UI text only (called internally)
    private void UpdateModeDisplay()
    {
        if (modeDisplay != null)
            modeDisplay.text = "Input Mode:" + GlobalInputMode + "\n Current Mode: " + currentMode;
    }

    // ----------- MOUSE MODE --------------

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightClickBehaviour();
        }

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        

        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()) { 
            if (Input.GetMouseButtonDown(0)) {
                RaycastSelect(cursorRay);
            }
            HoverSelect(cursorRay);
        }
    }

    private void RaycastSelect(Ray ray) {
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, rayLayers))
        {
            Debug.Log(rayHit.collider.tag);
            Debug.Log(rayHit.collider.name);
            GameManager.SelectionChanged?.Invoke();

            switch (rayHit.collider.tag)
            {
                case "Unit":
                    UnitClickBehaviour(rayHit.collider.GetComponentInParent<Unit>());
                    if (TutorialCursor != null)
                    {
                        TutorialCursor.NotifyUnitSelected();
                    }
                    break;

                case "Tile":
                    TileClickBehaviour(rayHit.collider.GetComponentInParent<Tile>());
                    break;

                case "Building":
                    BuildingClickBehaviour(rayHit.collider.GetComponent<Building>());
                    if (TutorialCursor != null)
                    {
                        TutorialCursor.NotifyBuildPerformed();
                    }
                    break;
            }
        }
    }

    private void HoverSelect(Ray ray) {
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, rayLayers)) {
            switch (rayHit.collider.tag) {
                case "Tile":
                    if (activeUnit != null && currentMode == UnitMode.Move) { 
                        activeUnit.HighlightMoveTile(rayHit.collider.GetComponentInParent<Tile>());
                    }
                    break;
            }
        }
    }

    public void HandSelectionStart()
    {
        if (!active)
        {
            return;
        }
        Ray fingerRay = new Ray(Fingertip.transform.position, Fingertip.transform.right);
        
        RaycastSelect(fingerRay);    
        //StartCoroutine(WaitToConfirm());
    }

    public void HandSelectionEnd()
    {        
        //StartCoroutine(WaitToDeselect());
    }

    //private IEnumerator WaitToConfirm()
    //{
    //    while (!canSelect) {
    //        yield return null;
    //    }
        
    //    canDeselect = false;
    //    handSelectionCheck = true;
    //    yield return new WaitForSeconds(1.5f);
    //    canDeselect = true;
    //}

    //private IEnumerator WaitToDeselect()
    //{
    //    while (!canDeselect) {
    //        yield return null;
    //    }
    //    canSelect = false;
    //    handSelectionCheck = false;
    //    yield return new WaitForSeconds(1.5f);
    //    canSelect = true;
    //}

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
        if (GlobalInputMode == InputMode.HandTracking && !canSwapMode) {
            return;
        }
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
        StartCoroutine(ModeSwapCooldown());
    }

    private IEnumerator ModeSwapCooldown()
    {
        canSwapMode = false;
        yield return new WaitForSeconds(2.5f);
        canSwapMode = true;
    }

    public void EndTurn()
    {
        ClearAll();
        GameManager.Instance.EndTurn(CurrentTeam);
        CurrentTeam = CurrentTeam == PlayerTeam.HUMAN ? PlayerTeam.ALIEN : PlayerTeam.HUMAN;
        GameManager.Instance.NewTurn(CurrentTeam);
    }
}
