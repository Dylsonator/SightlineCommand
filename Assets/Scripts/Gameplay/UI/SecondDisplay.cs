using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondDisplay : MonoBehaviour
{
    private void Awake()
    {
        int count = 0;
        foreach (var Disp in Display.displays)
        {
            count++;
            Disp.Activate(Disp.systemWidth, Disp.systemHeight, 60);
            if (count == 2)
            {
                break;
            }
        }
    }
}
