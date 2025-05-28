using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BuyableUnitEntry : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitStats;
    [SerializeField] private TextMeshProUGUI buyStats;
    private TutorialCursor tutorialCursor;

    private Unit unit;
    private BuyMenu buyMenu;
    private void Awake()
    {
        tutorialCursor = Camera.main.GetComponent<TutorialCursor>();
    }
    public void Initialize(Unit newUnit, BuyMenu source) {
        unit = newUnit;

        unitName.text = unit.displayName;
        unitStats.text = $"Attack: {unit.Damage}    Range: {unit.AttackRange}\n" +
            $"Health: {unit.MaxHealth}    Moves: {unit.MaxMovement}";
        buyStats.text = $"Cost:     {newUnit.tokenCost} tokens\nSpeed:  {newUnit.turnsToCreate} turn(s)";

        buyMenu = source;
    }

    public void OptionSelected() {
        buyMenu.UnitSelected(unit);
        if (tutorialCursor != null)
        {
            tutorialCursor.NotifyUnitHired();
        }
    }
}
