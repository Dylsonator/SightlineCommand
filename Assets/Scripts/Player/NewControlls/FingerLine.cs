using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FingerLine : MonoBehaviour
{
    public LineRenderer RayLine;
    public GameObject Fingertip;
    public Shader Highlight;

    private RaycastHit Hit;
    private Material originalMaterial;     //Cache original
    private Transform lastHitTransform;
    private MeshRenderer lastHitRenderer;
    private bool wasHittingLastFrame = false;

    private void Awake()
    {
        RayLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 origin;
        Vector3 direction;

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
        Debug.DrawRay(origin, direction * 500f, Color.red);

        bool didHit = Physics.Raycast(origin, direction, out Hit, 500f);

        if (didHit)
        {
            RayLine.SetPosition(1, Hit.point);

            Transform currentHit = Hit.transform;
            MeshRenderer currentRenderer = currentHit.GetComponent<MeshRenderer>();

            // If we're hitting a new object, reset the previous highlight
            if (lastHitTransform != null && lastHitTransform != currentHit)
            {
                ResetPreviousHighlight();
            }

            // Apply highlight to new object if needed
            if (currentRenderer != null && currentHit != lastHitTransform)
            {
                // Cache original material
                originalMaterial = currentRenderer.sharedMaterial;
                lastHitRenderer = currentRenderer;
                lastHitTransform = currentHit;

                // Create highlight material
                Material highlightMat = new Material(originalMaterial);
                highlightMat.shader = Highlight;

                highlightMat.SetFloat("_Selectable", currentHit.CompareTag("Border") ? 0 : 1);

                if (highlightMat.HasProperty("_BaseText"))
                {
                    highlightMat.SetTexture("_BaseText", originalMaterial.mainTexture);
                    
                }

                currentRenderer.material = highlightMat;
            }

            wasHittingLastFrame = true;
        }
        else
        {
            RayLine.SetPosition(1, origin + direction * 500f);

            // Reset highlight if no longer hitting anything
            if (wasHittingLastFrame)
            {
                ResetPreviousHighlight();
                wasHittingLastFrame = false;
            }
        }
    }

    private void ResetPreviousHighlight()
    {
        if (lastHitRenderer != null)
        {
            // Only reset if the material hasn't been changed by something else
            if (lastHitRenderer.material.shader == Highlight)
            {
                lastHitRenderer.sharedMaterial = originalMaterial;
            }
        }

        lastHitRenderer = null;
        lastHitTransform = null;
        originalMaterial = null;
    }

    public void SetActive(bool isActive)
    {
        if (RayLine != null)
            RayLine.enabled = isActive;

        if (!isActive)
        {
            ResetPreviousHighlight();
            wasHittingLastFrame = false;
        }

        this.enabled = isActive;
    }
}