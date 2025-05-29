using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float speed = 5;
    private float initialSpeed;
    private int direction = 1;
    private float scale;

    private void Awake() {
        initialSpeed = speed;
        CameraChange.CameraChanged += SwapDirection;
    }

    private void SwapDirection() {
        direction *= -1;
    }

    public void SetInitialPosition(float newScale) {
        scale = newScale;
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
