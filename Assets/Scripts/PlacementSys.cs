using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSys : MonoBehaviour
{
    [SerializeField] private GameObject objectIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField]
    private ObjDatabaseSO dataBase;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID){
        StopPlacement();
        selectedObjectIndex = dataBase.objsData.FindIndex(objInd => objInd.ID == ID);
        if(selectedObjectIndex < 0){
            Debug.Log($"No ID Found{ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceObjects;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceObjects() // if can be added to confirm where to place the object
    {
        if(inputManager.IsPointerOverUI()) return;
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        GameObject placeableItem = Instantiate(dataBase.objsData[selectedObjectIndex].prefab); //fetching prfab according to the objs id
        placeableItem.transform.position = grid.CellToWorld(cellPos); 
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceObjects;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if(selectedObjectIndex < 0) return;
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        objectIndicator.transform.position = touchPos;
        cellIndicator.transform.position = grid.CellToWorld(cellPos);  

    }
}
