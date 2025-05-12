using TMPro;
using UnityEngine;

/// <summary>
/// Nate
/// Behaviour of a building that can hire units.
/// Child class of Building, that can be activated to show a unit buy menu.
/// Inherited by OperatingBase.
/// </summary>
public class UnitCamp : Building {
    [Header("Units sold here")]
    public BuildingBuyMenu availableUnits;
    private TextMeshProUGUI createIndicator;

    [HideInInspector] public GameObject unitInCreation;
    [HideInInspector] public int turnsToCreate;

    protected override void Awake() {
        base.Awake();
        createIndicator = transform.Find("Canvas/Creation Indicator").GetComponent<TextMeshProUGUI>();
        createIndicator.gameObject.SetActive(false);
    }

    private void Start() {
        canActivate = true;
    }

    /// <summary>
    /// When a unit camp spawns, increase the unit token cap by 5 (add 5 more into circulation).
    /// </summary>
    public override void SpawnBehaviour() {
        base.SpawnBehaviour();
        if (GameManager.Instance != null && !GameManager.Instance.editorStart) {
            GameManager.Instance.AddTokens(team, 5);
        }
    }

    /// <summary>
    /// Show the buy menu when this is activated.
    /// </summary>
    /// <returns></returns>
    public override bool ActivateBehaviour() {
        GameManager.Instance.gameUI.ShowUnitBuyMenu(this);
        return true;
    }

    /// <summary>
    /// When a new turn starts, progresses creation of unit (if a unit is in creation).
    /// If the creation is done, spawns the new unit.
    /// </summary>
    public override void NewTurn() {
        base.NewTurn();
        if (unitInCreation != null) {
            turnsToCreate--;
            if (turnsToCreate <= 0) {
                GameObject unitSpawn = Instantiate(unitInCreation);
                unitSpawn.GetComponent<Unit>().UnitSpawn(tile);
                unitInCreation = null;
                createIndicator.gameObject.SetActive(false);
            } else {
                createIndicator.text = $"Unit Creating: {turnsToCreate} turn(s)";
            }
        }
    }

    /// <summary>
    /// When a unit is selected from the buy menu, try to buy it with tokens.
    /// If the transaction succeeded, start creating the unit.
    /// </summary>
    /// <param name="newUnit">The Unit script from the prefab to be made.</param>
    /// <returns>Whether the transaction was successful (is false if already making unit or didn't have enough tokens).</returns>
    public bool BuyUnit(Unit newUnit) {
        if (unitInCreation == null && GameManager.Instance.UseTokens(team, newUnit.tokenCost)) {
            unitInCreation = newUnit.gameObject;
            turnsToCreate = newUnit.turnsToCreate;
            createIndicator.gameObject.SetActive(true);
            createIndicator.text = $"Unit Creating: {turnsToCreate} turn(s)";
            return true;
        }
        return false;
    }
}
