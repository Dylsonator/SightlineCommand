using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(LineRenderer))] //forcing a linerenderer onto the object
public class FingerLine : MonoBehaviour //done by Dylan
{
    public LineRenderer RayLine;
    public GameObject Fingertip;
    public Shader Highlight;

    private RaycastHit Hit;
    private Material Origin;
    private Transform LastHit;

    private void Awake()
    {
        RayLine = GetComponent<LineRenderer>();
    }

    public void Update()
    {

        Vector3 OriginFinger = Fingertip.transform.position; //casting a raycast to set a line renderer where the player is pointing
        RayLine.SetPosition(0, Fingertip.transform.position);

        if (Physics.Raycast(OriginFinger, Fingertip.transform.right, out Hit, 500f))
        {
            RayLine.SetPosition(1, Hit.point);

            if (LastHit != null) { 
            MeshRenderer lastrenderer = LastHit.GetComponent<MeshRenderer>();
                if (lastrenderer != null && Origin != null) {
                    lastrenderer.material = Origin;
                }
            }
            MeshRenderer renderer = Hit.transform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Origin = renderer.material;
                Material HighlightMaterial = new Material(renderer.material);
                HighlightMaterial.shader = Highlight;

                if (HighlightMaterial.HasProperty("_BaseText"){
                    Texture originalTexture = Origin.mainTexture;
                    HighlightMaterial.SetTexture("_BaseText", originalTexture);
                }
                renderer.material = HighlightMaterial;
                    
                LastHit = Hit.transform;
            }

            Hit.transform.gameObject.GetComponent<MeshRenderer>().material.shader = Highlight;
        }
        else //
        {
            RayLine.SetPosition(1, OriginFinger + (Fingertip.transform.right * 500f));

            if (LastHit != null)
            {
                MeshRenderer lastRenderer = LastHit.GetComponent<MeshRenderer>();
                if (lastRenderer != null && Origin != null)
                {
                    lastRenderer.material = Origin;
                    
                }
                LastHit = null;
                Origin = null;                 
            }
        }
    }
}
