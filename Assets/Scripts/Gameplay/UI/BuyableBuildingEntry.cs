using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BuyableBuildingEntry : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buldingDescription;
    [SerializeField] private TextMeshProUGUI buildingCost;

    private Building building;
    private BuyMenu buyMenu;

    public void Initialize(Building newBuilding, BuyMenu source) {
        building = newBuilding;

        buildingName.text = building.buildingName;
        buldingDescription.text = building.toolTip;
        int buildingCostVal = building.price.costs[Mathf.Min(building.price.numberActive, building.price.costs.Length - 1)];
        buildingCost.text = $"{buildingCostVal} material";

        buyMenu = source;
    }

    public void OptionSelected() {
        buyMenu.BuildingSelected(building);
    }
}
