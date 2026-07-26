using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject scrollMenu;

    private void Start()
    {
        scrollMenu.SetActive(false);
    }
    public void OnTreeClicked()
    {
        scrollMenu.SetActive(true);
    }
    public void OnCloseClicked()
    {
        scrollMenu.SetActive(false);
    }
}
