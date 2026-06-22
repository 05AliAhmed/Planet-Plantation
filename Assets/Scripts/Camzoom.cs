using UnityEngine;

public class Camzoom : MonoBehaviour
{
    Animator camAnim;   
    void Start()
    {
        camAnim = GetComponent<Animator>();
        camAnim.Play("zoomin");
    }

}
