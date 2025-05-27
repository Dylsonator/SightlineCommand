using Leap;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HandCasting : MonoBehaviour
{ //Code originaly made by Nate/Edited into Hand Tracking System By Dylan
    public GameObject Fingertip;
    private Vector3 OriginFinger;

    public bool Check;
    public bool active;

    public void Selection()
    {
        if (!active)
        {
            return;
        }
        Vector3 OriginFinger = Fingertip.transform.position;
        RaycastHit FingerHit;
        Physics.Raycast(OriginFinger, Fingertip.transform.right, out FingerHit, 500f); //raycasting from finger to world
        //if (Check == true || Input.GetKeyDown(KeyCode.Space))
        //{

            Debug.Log(FingerHit.collider.tag);
            switch (FingerHit.collider.tag)
            {
                case "Unit":
                    UnitClickBehaviour(FingerHit.collider.GetComponentInParent<Unit>());//click behaviour are given values based on what is being selected
                break; 
                case "Tile":
                    TileClickBehaviour(FingerHit.collider.GetComponentInParent<Tile>());
                    break;
                case "Building":
                    BuildingClickBehaviour(FingerHit.collider.GetComponent<Building>());
                    break;
            }
        //}
    }
    public void Unitclick()
    {

    }
    public void Selections()
    {
        StartCoroutine(WaitToConfirm());
    }
    public void SelectEnd()
    {
        StartCoroutine(WaitToDeselect());
    }

    private IEnumerator WaitToConfirm()
    {
        Check = true;
        Debug.Log("True");
        yield return new WaitForSeconds(2.5f); //Delaying inputs
        Debug.Log("waited");
        
    }
    private IEnumerator WaitToDeselect()
    {
        Check = false;
        Debug.Log("false");
        yield return new WaitForSeconds(2.5f);
        Debug.Log("waited");

    }
    public virtual void UnitClickBehaviour(Unit unit) { }

    public virtual void TileClickBehaviour(Tile tile) { }

    public virtual void BuildingClickBehaviour(Building building) { } //more behaviours

    protected virtual void TileHoverBehaviour(Tile tile) { }

    protected virtual void RightClickBehaviour() { }
}
