using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyMenu : MonoBehaviour {
    [SerializeField]
    private GameObject buyableUnitEntry;

    [SerializeField]
    private GameObject buyableBuildingEntry;

    [SerializeField]
    private Transform scrollContent;

    [SerializeField]
    private TextMeshProUGUI errorIndicator;

    private List<GameObject> entries = new List<GameObject>();

    public UnitCamp building;
    public Unit unit;
    private TutorialCursor tutorialCursor;

    private void Awake()
    {
        tutorialCursor = Camera.main.GetComponent<TutorialCursor>();
    }
    public void InitializeBuilding(UnitCamp source) {
        building = source;
        for (int i = 0; i < building.availableUnits.units.Count; i++) { 
            GameObject newEntry = Instantiate(buyableUnitEntry);

            newEntry.GetComponent<BuyableUnitEntry>().Initialize(building.availableUnits.units[i], this);

            SetEntryPosition(newEntry.GetComponent<RectTransform>(), i);
        }
    }

    public void InitializeUnit(Unit source) {
        unit = source;
        for (int i = 0; i < unit.createableBuildings.Count; i++) {
            GameObject newEntry = Instantiate(buyableBuildingEntry);

            newEntry.GetComponent<BuyableBuildingEntry>().Initialize(unit.createableBuildings[i], this);

            SetEntryPosition(newEntry.GetComponent<RectTransform>(), i);
        }
    }

    private void SetEntryPosition(RectTransform entry, int i) {
        entry.SetParent(scrollContent, true);
        Vector2 entryPos = new Vector2(0, -90 + i * -175);

        entry.localPosition = entryPos;
        entry.localScale = new Vector3(1, 1, 1);
        entries.Add(entry.gameObject);
    }

    public void UnitSelected(Unit newUnit) {
        if (building.BuyUnit(newUnit)) {
            GameManager.Instance.gameUI.HideUnitBuyMenu();
        } else if (building.unitInCreation != null) {
             StartCoroutine(ShowError("Already creating a unit!"));
        } else {
            StartCoroutine(ShowError("Insufficient Troop Tokens!"));
        }
    }

    public void BuildingSelected(Building building) {
        if (unit.CreateBuilding(building)) {
            if (tutorialCursor != null)
            {
                tutorialCursor.NotifyBuildingCreate();
            }
            GameManager.Instance.gameUI.HideBuildingBuyMenu();
        } else {
            StartCoroutine(ShowError("Insufficient Materials!"));
        }
    }

    private IEnumerator ShowError(string error) {
        errorIndicator.text = error;
        yield return new WaitForSeconds(1);
        errorIndicator.text = "";
    }


    public void HideMenu() {
        foreach (var entry in entries) {
            Destroy(entry);
        }
        entries.Clear();
        StopAllCoroutines();
        errorIndicator.text = "";
        building = null;
        unit = null;
        //gameObject.SetActive(false);
    }
}
