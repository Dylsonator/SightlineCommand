using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool open = false;
    public GameObject canvas;
    public GameObject[] OtherCanvas;
    KeyCode esc = KeyCode.Escape;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(esc)) { 
        open = !open;
        }
        if (open)
        {
            canvas.SetActive(true);
            foreach (GameObject obj in OtherCanvas) { 
            obj.SetActive(false);
            }
        }
        if (!open)
        {
            canvas.SetActive(false);
            foreach (GameObject obj in OtherCanvas)
            {
                obj.SetActive(true);
            }
        }
    }

}
