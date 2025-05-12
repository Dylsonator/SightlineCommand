using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))] //forcing a linerenderer onto the object
public class FingerLine : MonoBehaviour //done by Dylan
{
    public LineRenderer RayLine;
    public GameObject Fingertip;
    private RaycastHit Hit;
    
    private void Awake()
    {
        RayLine = GetComponent<LineRenderer>();        
        
    }

    public void Update() {

        Vector3 OriginFinger = Fingertip.transform.position; //casting a raycast to set a line renderer where the player is pointing
        RayLine.SetPosition(0, Fingertip.transform.position);      

        if (Physics.Raycast(OriginFinger, Fingertip.transform.right, out Hit, 500f))
        {
            RayLine.SetPosition(1, Hit.point);
        }
        else //
        {
            RayLine.SetPosition(1, OriginFinger + (Fingertip.transform.right * 500f)); 
        }
    }
}
