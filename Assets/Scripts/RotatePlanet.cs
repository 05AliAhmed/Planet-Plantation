using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    [SerializeField] float rotationSensi = 0.02f;
    [SerializeField] float rotationSpeed = 0.2f; // controls how fast input affects rotation
    [SerializeField] float damping = 5f; // controls how quickly rotation slows down (higher = stops faster)

    private float rotXVelocity; // stores horizontal rotation speed (left/right drag)
    private float rotYVelocity; // stores vertical rotation speed (up/down drag)

    // MOBILE INPUTS
    private float rotY;
    private float rotX;

    void Update()
    {
        // // CHECK IF PLAYER IS DRAGGING (HOLDING LEFT MOUSE BUTTON)
        // if (Input.GetMouseButton(0))
        // {
        //     // Mouse X = horizontal movement, Mouse Y = vertical movement
        //     float inputX = Input.GetAxis("Mouse X");
        //     float inputY = Input.GetAxis("Mouse Y");
        //     // multiplied to increase sensitivity
        //     rotXVelocity = inputX * rotationSpeed * 100f;
        //     rotYVelocity = inputY * rotationSpeed * 100f;
        // }
        // else
        // {
        //     // gradually reduce velocity to 0 (this creates inertia effect)
        //     rotXVelocity = Mathf.Lerp(rotXVelocity, 0, damping * Time.deltaTime);
        //     rotYVelocity = Mathf.Lerp(rotYVelocity, 0, damping * Time.deltaTime);
        // }

        // transform.Rotate(Vector3.up, -rotXVelocity, Space.World);
        // transform.Rotate(Vector3.right, -rotYVelocity, Space.Self);

        // Mobile input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // checking for taps on screen

            if (touch.phase == TouchPhase.Moved) // checking for drag
            {
                rotX = touch.deltaPosition.x * rotationSpeed * rotationSensi;
                rotY = touch.deltaPosition.y * rotationSpeed * rotationSensi;
            }
            else //if(touch.phase == TouchPhase.Ended)
            {
                // gradually reduce velocity to 0 (this creates inertia effect)
                rotX = Mathf.Lerp(rotX, 0, damping * Time.deltaTime);
                rotY = Mathf.Lerp(rotY, 0, damping * Time.deltaTime);
            }
            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, -rotY, Space.World);
        }
    }
}

