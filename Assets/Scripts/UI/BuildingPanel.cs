using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingPanel : MonoBehaviour {

    [SerializeField]
    private GameObject buildingUnitEntry;

    [SerializeField]
    private Transform scrollContent;

    [SerializeField]
    private HealthBar healthBar;

    private GameUI gameUI;

    private Building building;
    private List<GameObject> entries = new List<GameObject>();

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI tipText;
    private Transform activeButton;
    private TextMeshProUGUI commandText;

    private void Awake() {
        gameUI = GetComponentInParent<GameUI>();
        nameText = transform.Find("BuildingName").GetComponent<TextMeshProUGUI>();
        tipText = transform.Find("ToolTip").GetComponent<TextMeshProUGUI>();
        activeButton = transform.Find("ActiveButton");
        commandText = activeButton.Find("Action").GetComponent<TextMeshProUGUI>();
        HidePanel();
    }

    public void OnEnable() {
        GameManager.SelectionChanged += HidePanel;
    }

    public void OnDisable() {
        GameManager.SelectionChanged -= HidePanel;
    }

    public void SetBuilding(Building selectedBuilding) {
        //gameUI.ShowBuildingPanel();
        building = selectedBuilding;
        nameText.text = building.buildingName;
        tipText.text = building.toolTip;
        UnitCamp buildCamp = building as UnitCamp;
        if (buildCamp != null && buildCamp.unitInCreation != null) {
            tipText.text += $"\nHiring: {buildCamp.unitInCreation.GetComponent<Unit>().displayName}, complete in {buildCamp.turnsToCreate} turn(s)";
        }
        activeButton.gameObject.SetActive(building.canActivate);
        if (building.canActivate) {
            commandText.text = building.command;
        }
        
        for (int i = 0; i < building.unitsHere.Count; i++) {
            GameObject newEntry = Instantiate(buildingUnitEntry);
            newEntry.transform.SetParent(scrollContent, true);

            Vector2 entryPos = new Vector2(181.381424f, -75 + i * -125);

            Unit unit = building.unitsHere[i].GetComponent<Unit>();
            newEntry.GetComponent<BuildingUnitEntry>().Initialize(unit, building, this, entryPos);
            entries.Add(newEntry);
        }
        healthBar.gameObject.SetActive(true);
        healthBar.DisplaySpecified(building.maxHealth, building.health, building.team, true);
    }
    
    public void HidePanel() {
        nameText.text = ("Building");
        tipText.text = ("No Building Selected");
        building = null;
        foreach (GameObject entry in entries) {
            Destroy(entry);
        }
        entries.Clear();
        healthBar.gameObject.SetActive(false);
        activeButton.gameObject.SetActive(false);
        //gameUI.HideBuildingPanel();
    }

    public void ActivateBuilding()
    {
        if (building != null)
        {
            if (building.ActivateBehaviour())
            {
                HidePanel();
            }
        }
    }

}
