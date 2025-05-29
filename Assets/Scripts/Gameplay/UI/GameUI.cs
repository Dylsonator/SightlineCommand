using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameUI : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI modeDisplay;
    [SerializeField]
    private TextMeshProUGUI teamDisplay;
    [SerializeField]
    private TextMeshProUGUI statsDisplay;
    [SerializeField]
    private TextMeshProUGUI winDisplay;
    [SerializeField]
    private BuyMenu buyMenu;


    private UnifiedCursor gameCursor;

    [Header("Team UI")]
    [SerializeField]
    private TeamUIParams humanUI;
    [SerializeField]
    private TeamUIParams alienUI;
    [SerializeField]
    private Image[] teamBackgrounds;
    [SerializeField]
    private TextMeshProUGUI[] teamText;
    [SerializeField]
    private Image[] teamButtons;

    private GameObject turnPanel;
    public GameObject buildingPanel;
    [SerializeField]
    private GameObject winPanel;
    

    public void UpdateModeDisplay(int modeIndex) {
        gameCursor.SetBehaviour(modeIndex);
        modeDisplay.text = $"Current mode: {gameCursor.currentMode}";
    }

    public void UpdateTeamDisplay() {
        TeamUIParams team = gameCursor.CurrentTeam == PlayerTeam.HUMAN ? humanUI : alienUI;
        teamDisplay.text = $"Team: {team.teamName}";
        foreach (var background in teamBackgrounds) {
            background.sprite = team.background;
        }
        foreach (var text in teamText) {
            text.color = team.textColor;
        }
        foreach (var button in teamButtons) {
            button.color = team.buttonColor;
        }
        UpdateStats();
    }

    public void UpdateStats() {
        int material = GameManager.Instance.players[gameCursor.CurrentTeam].material;
        int tokens = GameManager.Instance.players[gameCursor.CurrentTeam].troopTokens;
        statsDisplay.text = $"Material: {material}\nUnit Tokens: {tokens}";
    }

    public void EndTurn() {
        gameCursor.EndTurn();
        UpdateTeamDisplay();
    }

    public void ShowBuildingPanel() {
        buildingPanel.SetActive(true);
        turnPanel.SetActive(false);
    }

    public void HideBuildingPanel() {
        buildingPanel.SetActive(false);
        turnPanel.SetActive(true);
    }

    public void GameStart() {
        turnPanel = transform.Find("Turn Panel").gameObject;
        //winPanel = transform.Find("Game End Panel").gameObject;
        gameCursor = GameManager.Instance.gameCursor;
        turnPanel.SetActive(true);
        //buildingPanel.SetActive(false);
        winPanel.SetActive(false);
        UpdateTeamDisplay();
    }

    public void ShowUnitBuyMenu(UnitCamp source) {
        //buyMenu.gameObject.SetActive(true);
        buyMenu.InitializeBuilding(source);
        //buildingPanel.gameObject.SetActive(false);
        GameManager.SelectionChanged += HideUnitBuyMenu;
    }

    public void HideUnitBuyMenu() {
        GameManager.SelectionChanged -= HideUnitBuyMenu;
        //buildingPanel.gameObject.SetActive(true);
        buyMenu.HideMenu();
    }

    public void ShowBuildingBuyMenu(Unit source) {
        //buyMenu.gameObject.SetActive(true);
        buyMenu.InitializeUnit(source);
        //turnPanel.gameObject.SetActive(false);
        GameManager.SelectionChanged += HideBuildingBuyMenu;
    }

    public void HideBuildingBuyMenu() {
        GameManager.SelectionChanged -= HideBuildingBuyMenu;
        //turnPanel.gameObject.SetActive(true);
        buyMenu.HideMenu();
    }

    public void HideAnyBuyMenu() {
        if (buyMenu.building != null) {
            HideUnitBuyMenu();
        } else if (buyMenu.unit != null) { 
            HideBuildingBuyMenu();
        }
    }

    public void DisplayGameOver(PlayerTeam defeatedTeam) {
        turnPanel.SetActive(false);
        winPanel.SetActive(true);
        switch (defeatedTeam) {
            case PlayerTeam.HUMAN:
                winDisplay.text = "Aliens Win!";
                break;
            case PlayerTeam.ALIEN:
                winDisplay.text = "Humans Win!";
                break;
        }
    }

    public void RestartGame() {
        GameManager.Instance.RestartGame();
    }
}
