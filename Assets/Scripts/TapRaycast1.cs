using UnityEngine;
using UnityEngine.SceneManagement;

public class TapRaycast1 : MonoBehaviour
{
    private Vector2 startPos;
    private float tapThreshold = 10f; // pixels

    void Update()
    {
        if(Input.touchCount > 0){

            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began){
                startPos = touch.position;    
            }

            if(touch.phase == TouchPhase.Ended){
                float distance = Vector2.Distance(startPos, touch.position);
                if (distance < tapThreshold){
                    HandleTap(touch.position);
                }    
            }
        }
    }
    void HandleTap(Vector2 tapPos) // loading area on tap
    {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.collider.CompareTag("Meadow")){
                SceneManager.LoadScene(2);
            }
        }
    }
}
