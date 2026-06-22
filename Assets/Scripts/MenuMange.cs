using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMange : MonoBehaviour
{
    public void changeScn(int _index){
        SceneManager.LoadScene(_index);
    }
}
