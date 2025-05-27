using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorial : MonoBehaviour
{
    public void EndTutor()
    {
        PlayerPrefs.SetInt("FirstTimeCheck", 1);
        SceneManager.LoadScene(0);
    }
}
