using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public string instructionText;
    public bool allowClick = false;
    public GameObject requiredClickTarget = null;
    public bool waitForKey = false;
    public KeyCode requiredKey = KeyCode.None;
}