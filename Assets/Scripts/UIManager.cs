using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject scrollMenu;
    // [SerializeField] private GameObject gridVisualization;

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

    // public override void StopPlacement()
    // {
    //     base.StopPlacement();
    //     scrollMenu.SetActive(false);
    // }
}
