using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public LayerMask placementLayerMask;
    public Camera scnCam;
    private Vector3 lastPos;
    private Vector2 strtPos;
    private float tapThreshold = 10f;

    public event Action OnClicked, OnExit;

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
