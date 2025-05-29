using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float speed = 5;
    private float initialSpeed;
    private int direction = 1;
    private float scale;

    private Vector2 xBounds;
    private Vector2 zBounds;

    private void Awake() {
        initialSpeed = speed;
        CameraChange.CameraChanged += SwapDirection;
    }

    private void SwapDirection() {
        direction *= -1;
    }

    public void SetInitialPosition(float newScale, Vector2 newXBounds, Vector2 newZBounds) {
        scale = newScale;
        xBounds = newXBounds;
        zBounds = newZBounds;
        Vector3 newPos = transform.position;
        newPos.y = scale * 10;
        speed = initialSpeed * scale;

    }

    private void Update() {
        sprint();

        Vector3 newPos = transform.position;
        //this is not a neat way of doing this so I might change it later but it works for now.
        float difference = Time.deltaTime * speed * direction;
        if (Input.GetKey(KeyCode.W)) {
            newPos.z += difference;
        }
        if (Input.GetKey(KeyCode.A)) {
            newPos.x -= difference;
        }
        if (Input.GetKey(KeyCode.S)) {
            newPos.z -= difference;
        }
        if (Input.GetKey(KeyCode.D)) {
            newPos.x += difference;
        }
        if (Input.GetKey(KeyCode.Q)) {
            newPos.y -= Time.deltaTime * speed; 
        }   
        if (Input.GetKey(KeyCode.E)) {
            newPos.y += Time.deltaTime * speed;
        }
        newPos.x = Mathf.Clamp(newPos.x, xBounds.x, xBounds.y);
        newPos.z = Mathf.Clamp(newPos.z, zBounds.x, zBounds.y);
        newPos.y = Mathf.Clamp(newPos.y, 5, 15);

        transform.position = newPos;
    }

    private void sprint() //Added By Dylan
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 25 * scale;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 5 * scale;
        }
    }
}
