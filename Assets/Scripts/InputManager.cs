using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
    }

    private void Update()
    {
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) strtPos = touch.position;
            if(touch.phase == TouchPhase.Ended){
                float distance = Vector2.Distance(strtPos, touch.position);
                if(distance < tapThreshold){
                        OnClicked?.Invoke(); // now on draging shouhld  be no obj  placements, working
                }
            }
            // CAMERA MOVEMENT (DRAG)
            if (touch.phase == TouchPhase.Moved && isPlacing == false)
            {
                Vector2 delta = touch.deltaPosition;
                move = new Vector3(-delta.x, 0, -delta.y) * moveSpeed * Time.deltaTime;
                Vector3 newPos = scnCam.transform.position + move;
                // newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                // newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
                if (newPos.x >= minX && newPos.x <= maxX){ //checks if within bounds then move
                        scnCam.transform.position += new Vector3(move.x, 0, 0);
                    }
                    // Check Z movement
                if (newPos.z >= minZ && newPos.z <= maxZ){
                        scnCam.transform.position += new Vector3(0, 0, move.z);
                    }
                // scnCam.transform.position += move;
                // scnCam.transform.position = newPos;
            }
        }
        if (move.magnitude > 0.001f) // inertia effect
        {
            scnCam.transform.position += move;
            // Smoothly slow down movement
            move = Vector3.Lerp(move, Vector3.zero, damping * Time.deltaTime);
        }
        if(Input.GetKeyDown(KeyCode.Escape)) // need to work on this on button tap or something to go back, now working used escape btn to call stop placement directly
            OnExit?.Invoke();

    
    }

    public bool IsPointerOverUI(){
        return EventSystem.current.IsPointerOverGameObject(); 
    }

    public Vector3 PosOnGrid(){
        if(Input.touchCount > 0)
        {
            Vector3 touchPosition = Input.GetTouch(0).position;
            // touchPosition.z = scnCam.nearClipPlane;
            Ray ray = scnCam.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, placementLayerMask)){
                lastPos = hit.point;
            }
        }
        return lastPos;
    }
}
