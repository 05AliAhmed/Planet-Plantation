using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TapRaycast1 : MonoBehaviour
{
    private Vector2 startPos;
    private float tapThreshold = 10f; // pixels 
    public TMP_Text areaName;

    public float rayCastCorX;
    public float rayCastCorY;
    public float rayLenght;


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
        AreaNameRay();
    }
    void HandleTap(Vector2 tapPos) // loading area on tap
    {
        Ray ray = Camera.main.ScreenPointToRay(tapPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Meadow")) SceneManager.LoadScene(2);
            if (hit.collider.CompareTag("Urban")) SceneManager.LoadScene(3);
            if (hit.collider.CompareTag("Jungle")) SceneManager.LoadScene(4);
            if (hit.collider.CompareTag("Stoneage")) SceneManager.LoadScene(5);
        }
    }

    void AreaNameRay() // Displaying area name
    {
        Ray areaCheck = Camera.main.ScreenPointToRay(new Vector3(rayCastCorX, rayCastCorY, 0));
        RaycastHit hit;

            // Draw the ray in Scene view
        Debug.DrawRay(
            areaCheck.origin,              // starting point of ray
            areaCheck.direction * rayLenght,    // direction and length of ray
            Color.red                      // color of ray
        );

        if(Physics.Raycast(areaCheck, out hit))
        {
            if (hit.collider.CompareTag("Meadow")) areaName.text = "Meadow";
            else if (hit.collider.CompareTag("Urban")) areaName.text = "Urban";
            else if (hit.collider.CompareTag("Jungle")) areaName.text = "Jungle";
            else if (hit.collider.CompareTag("Stoneage")) areaName.text = "Stone Age Village";
            else                                          areaName.text = "";
        } 
        else areaName.text = "";
    }  
}
