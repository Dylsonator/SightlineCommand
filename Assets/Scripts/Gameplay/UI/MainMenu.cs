using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour //made by Dylan
{
    private int FirstGame = 0;
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    } //basic loading scenes depending on buttons
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Awake()
    {
        FirstGame = PlayerPrefs.GetInt("FirstTimeCheck"); //checks if the first time opening the game to force player into tutorial
        if (FirstGame == 0)
        {
            PlayerPrefs.SetInt("FirstTimeCheck", 1); //first time check done here
            SceneManager.LoadScene(2);
            
        }
    }

    public void Level1()
    {
        SceneManager.LoadScene(3);
        }
    public void Level2()
    {
        SceneManager.LoadScene(4);
    }
    public void Level3()
    {
        SceneManager.LoadScene(5);
    }//basic loading scenes depending on buttons
    public void MainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void TutorialScene()
    {
        SceneManager.LoadScene(2);
    }


}
