using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCanvas : MonoBehaviour
{
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    
    }
    private void OnMouseOver()
    {
        Debug.Log("Entered");
        canvas.enabled = true;
    }

    void OnMouseExit() 
    {
        Debug.Log("Exited");
        canvas.enabled = false;
    }
}
