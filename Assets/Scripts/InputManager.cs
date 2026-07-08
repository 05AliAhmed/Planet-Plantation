using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public LayerMask placementLayerMask;
    public Camera scnCam;
    private Vector3 lastPos;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    // public bool IsPointerOverUI() // returns t if tap is over ui of objs n f if over 
    //     => EventSystem.current.IsPointerOverGameObject();
    public bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 PosOnGrid(){
        Vector3 touchPosition = Input.mousePosition;
        touchPosition.z = scnCam.nearClipPlane;
        Ray ray = scnCam.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask)){
            lastPos = hit.point;
        }
        return lastPos;
    }
}
