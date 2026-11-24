using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject scrollMenu;
    [SerializeField] public PlacementSys placementSys;
    // [SerializeField] private GameObject gridVisualization;

    private void Start()
    {
        scrollMenu.SetActive(false);
        placementSys = GameObject.Find("PlacementSys").GetComponent<PlacementSys>();
    }
    public void OnTreeClicked()
    {
        scrollMenu.SetActive(true);
        GameState.PanelOpen = true;
    }
    public void OnCloseClicked()
    {
        scrollMenu.SetActive(false);
        placementSys.StopPlacement();
        GameState.PanelOpen = false;
    }

    private void OnDestroy()
    {
        GameState.PanelOpen = false;   // in UIManager
        GameState.Placing = false;     // in PlacementSys
    }

    //public override void StopPlacement()
    //{
    //    base.StopPlacement();
    //    scrollMenu.SetActive(false);
    //}
}
