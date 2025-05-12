using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameHandsUI : MonoBehaviour
{
    //Code made by Nate/Edited into Hand Tracking System By Dylan

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


    private HandCursor handCursor;

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
    private GameObject winPanel;
    private int CamAngle;
    

    public void UpdateModeDisplay(int modeIndex) {
        handCursor.SetBehaviour(modeIndex);
        modeDisplay.text = $"Current mode: {handCursor.currentMode}";
    }

    public void UpdateTeamDisplay() {
        TeamUIParams team = handCursor.CurrentTeam == PlayerTeam.HUMAN ? humanUI : alienUI; //changing the ui based on team
        teamDisplay.text = $"Team: {team.teamName}";
        foreach (var background in teamBackgrounds) {
            background.sprite = team.background;
        }
        foreach (var text in teamText) { //finds each text and button to change colours
            text.color = team.textColor;
        }
        foreach (var button in teamButtons) {
            button.color = team.buttonColor;
        }
        UpdateStats();
    }

    public void UpdateStats() {
        int material = GameManager.Instance.players[handCursor.CurrentTeam].material;
        int tokens = GameManager.Instance.players[handCursor.CurrentTeam].troopTokens;
        statsDisplay.text = $"Material: {material}\nUnit Tokens: {tokens}"; //updates the stats for materials and unit tokens when needed
    }

    public void EndTurn()
    {
        handCursor.EndTurn();
        UpdateTeamDisplay();
    }

    //public void ShowBuildingPanel() {
    //    buildingPanel.SetActive(true);
    //    turnPanel.SetActive(false);
    //}

    //public void HideBuildingPanel() {
    //    buildingPanel.SetActive(false);
    //    turnPanel.SetActive(true);
    //}

    public void GameStart() {
        turnPanel = transform.Find("Turn Panel").gameObject; //game start setups
        winPanel = transform.Find("Game End Panel").gameObject;
        handCursor = GameObject.Find("GhostHands").GetComponent<HandCursor>();
        turnPanel.SetActive(true);
        //buildingPanel.SetActive(false);
        winPanel.SetActive(false);
        UpdateTeamDisplay();
    }

    public void ShowBuyMenu(UnitCamp source) {
        buyMenu.gameObject.SetActive(true);
        buyMenu.InitializeBuilding(source);
        //buildingPanel.gameObject.SetActive(false); //Redundant
    }

    public void HideBuyMenu() {
        buildingPanel.gameObject.SetActive(true); //redundant 
        //buyMenu.HideMenu();
    }

    public void DisplayGameOver(PlayerTeam defeatedTeam) {
        turnPanel.SetActive(false);
        winPanel.SetActive(true);
        switch (defeatedTeam) {
            case PlayerTeam.HUMAN:
                winDisplay.text = "Aliens Win!";
                break; //ui change to who wins
            case PlayerTeam.ALIEN:
                winDisplay.text = "Humans Win!";
                break;
        }
    }

    public void RestartGame() {
        GameManager.Instance.RestartGame();
    }
}
