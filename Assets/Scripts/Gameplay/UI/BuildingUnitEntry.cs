using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;

public class BuildingUnitEntry : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI unitType;

    [SerializeField]
    private TextMeshProUGUI currentHP;

    private Unit unit;
    private Building building;

    private HandCursor gameCursor;
    private BuildingPanel buildPanel;
    private RectTransform rect;

    private void Start() {
        gameCursor = GameManager.Instance.gameCursor;
        rect = GetComponent<RectTransform>();
        
    }

    public void Initialize(Unit thisUnit, Building thisBuilding, BuildingPanel bp, Vector2 position) { 
        unit = thisUnit;
        unitType.text = unit.displayName;
        currentHP.text = $"HP: {unit.Health}/{unit.MaxHealth}";
        building = thisBuilding;
        buildPanel = bp;
        
        StartCoroutine(DelayPosition(position));
    }

    //Due to a bug, the first time the entries are displayed, they are set to the wrong position.
    //This coroutine delays the action of setting its position by a frame, circumventing this issue.
    private IEnumerator DelayPosition(Vector2 pos)
    {
        yield return null;
        rect.localPosition = pos;
    }

    public void SelectUnit() {
        building.OnExitBehaviour(unit);        
        buildPanel.HidePanel();
        gameCursor.UnitClickBehaviour(unit);
    }
}
