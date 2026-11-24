using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Readme;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float damping = 0.3f;
    [SerializeField] private float padding = 5f;
    [SerializeField] private Transform ground;
    public LayerMask placementLayerMask;
    public Camera scnCam;
    private Vector3 lastPos;
    // private float moveX;
    private Vector2 strtPos;
    private Vector3 move;
    private float tapThreshold = 10f;

    private float minX; float maxX; float minZ; float maxZ;
    public bool isPlacing;


    public event Action OnClicked, OnExit;

    private void Start()
    {
        isPlacing = false;
        Renderer rend = ground.GetComponent<Renderer>();
        Vector3 size = rend.bounds.size;
        Vector3 center = rend.bounds.center;
        minX = (center.x - size.x / 2) + padding;
        maxX = (center.x + size.x / 2) - padding;
        minZ = (center.z - size.z / 2) + padding;
        maxZ = (center.z + size.z / 2) - padding;

        // [NEW] if padding is bigger than half the ground, pin the camera to the centre
        // instead of letting min > max (Mathf.Clamp would behave oddly)
        if (minX > maxX) minX = maxX = center.x;
        if (minZ > maxZ) minZ = maxZ = center.z;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                strtPos = touch.position;
                move = Vector3.zero;        // [NEW] a new touch stops any leftover inertia
            }
            if (touch.phase == TouchPhase.Ended)
            {
                float distance = Vector2.Distance(strtPos, touch.position);
                if (distance < tapThreshold)
                {
                    OnClicked?.Invoke(); // now on draging shouhld  be no obj  placements, working
                }
            }
            // CAMERA MOVEMENT (DRAG)
            if (touch.phase == TouchPhase.Moved && isPlacing == false)
            {
                Vector2 delta = touch.deltaPosition;
                move = new Vector3(-delta.x, 0, -delta.y) * moveSpeed * Time.deltaTime;
                MoveCameraClamped(move);    // [CHANGED] one clamped move, applied once
            }
        }
        else if (move.magnitude > 0.001f) // inertia effect, [CHANGED] only when no finger is down
        {
            MoveCameraClamped(move);        // [CHANGED] clamped too, so inertia can't push past the edge
            // Smoothly slow down movement
            move = Vector3.Lerp(move, Vector3.zero, damping * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // need to work on this on button tap or something to go back, now working used escape btn to call stop placement directly
            OnExit?.Invoke();


    }

    // [NEW] every camera move goes through here, so the camera can never leave the bounds
    private void MoveCameraClamped(Vector3 delta)
    {
        Vector3 p = scnCam.transform.position + delta;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.z = Mathf.Clamp(p.z, minZ, maxZ);
        scnCam.transform.position = p;
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 PosOnGrid()
    {
        if (Input.touchCount > 0)
        {
            Vector3 touchPosition = Input.GetTouch(0).position;
            // touchPosition.z = scnCam.nearClipPlane;
            Ray ray = scnCam.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
            {
                lastPos = hit.point;
            }
        }
        return lastPos;
    }
}