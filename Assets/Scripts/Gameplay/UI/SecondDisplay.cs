using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondDisplay : MonoBehaviour
{
    private void Awake()
    {
        foreach (var Disp in Display.displays)
        {
            Disp.Activate(Disp.systemWidth, Disp.systemHeight, 60);
        }
    }
}
