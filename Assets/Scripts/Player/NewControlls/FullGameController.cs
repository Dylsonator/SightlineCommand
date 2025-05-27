using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputMode
{
    Mouse,
    HandTracking
}
public class FullGameController : MonoBehaviour
{
    public GameCursor mouseCursor;
    public HandCursor handCursor;

    private InputMode currentInputMode = InputMode.Mouse;

    private void Start()
    {
        SetInputMode(currentInputMode);
    }
    private void Update()
    {
        // Temp toggle input: press T to toggle input mode
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentInputMode = currentInputMode == InputMode.Mouse ? InputMode.HandTracking : InputMode.Mouse;
            SetInputMode(currentInputMode);
            Debug.Log("Switched to " + currentInputMode);
        }
    }
    void SetInputMode(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.Mouse:
                mouseCursor.enabled = true;
                handCursor.enabled = false;
                break;

            case InputMode.HandTracking:
                mouseCursor.enabled = false;
                handCursor.enabled = true;
                break;
        }
    }
}
