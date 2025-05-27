using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(LineRenderer))]
public class MouseHighlihgtLine : MonoBehaviour
{
    public LineRenderer RayLine;
    public GameObject Fingertip;
    public Shader Highlight;

    private RaycastHit Hit;
    private Material Origin;
    private Transform LastHit;
    private MeshRenderer lastRenderer;

    private void Awake()
    {
        RayLine = GetComponent<LineRenderer>();

        // Disable if not in HandTracking mode
        if (UnifiedCursor.GlobalInputMode != UnifiedCursor.InputMode.HandTracking)
        {
            this.SetActive(false);
        }
    }

    public void Update()
    {
        Vector3 origin;
        Vector3 direction;

        // Switch between HandTracking and Mouse mode
        if (UnifiedCursor.GlobalInputMode == UnifiedCursor.InputMode.HandTracking)
        {
            origin = Fingertip.transform.position;
            direction = Fingertip.transform.right;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            origin = ray.origin;
            direction = ray.direction;
        }

        RayLine.SetPosition(0, origin);

        if (Physics.Raycast(origin, direction, out Hit, 500f))
        {
            RayLine.SetPosition(1, Hit.point);

            if (LastHit != null)
            {
                lastRenderer = LastHit.GetComponent<MeshRenderer>();
                if (lastRenderer != null && Origin != null)
                {
                    lastRenderer.material = Origin;
                }
            }

            MeshRenderer renderer = Hit.transform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Origin = renderer.material;
                Material highlightMaterial = new Material(renderer.material);
                highlightMaterial.shader = Highlight;

                if (highlightMaterial.HasProperty("_BaseText"))
                {
                    Texture originalTexture = Origin.mainTexture;
                    highlightMaterial.SetTexture("_BaseText", originalTexture);
                }

                renderer.material = highlightMaterial;
                LastHit = Hit.transform;
            }

            if (Hit.transform.parent != null && Hit.transform.parent.GetComponent<Tile>() != null)
            {
                Hit.transform.GetComponent<MeshRenderer>().material.shader = Highlight;
            }
        }
        else
        {
            RayLine.SetPosition(1, origin + direction * 500f);

            if (LastHit != null)
            {
                lastRenderer = LastHit.GetComponent<MeshRenderer>();
                if (lastRenderer != null && Origin != null)
                {
                    lastRenderer.material = Origin;
                }

                LastHit = null;
                Origin = null;
            }
        }
    }

    public void SetActive(bool isActive)
    {
        if (RayLine != null)
            RayLine.enabled = isActive;

        if (lastRenderer != null && Origin != null)
        {
            lastRenderer.material = Origin;
        }

        this.enabled = isActive;
    }
}
