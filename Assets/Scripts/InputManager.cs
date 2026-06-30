using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask placementLayerMask;
    public Camera scnCam;
    private Vector3 lastPos;

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
