using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlacementSys : MonoBehaviour
{
    [SerializeField] private GameObject objectIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField]
    private ObjDatabaseSO dataBase;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData plantsData;
    private List<GameObject> placedGameObjs = new();

    [SerializeField]
    private PreviewSys preview;

    private Vector3Int lastDetectedPos = Vector3Int.zero;

    private void Start()
    {
        StopPlacement();
        plantsData = new();
    }

    public void StartPlacement(int ID){
        StopPlacement();
        selectedObjectIndex = dataBase.objsData.FindIndex(objInd => objInd.ID == ID);
        if(selectedObjectIndex < 0){
            Debug.Log($"No ID Found{ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(dataBase.objsData[selectedObjectIndex].prefab,
                                            dataBase.objsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceObjects;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceObjects() // if can be added to confirm where to place the object
    {
        if(inputManager.IsPointerOverUI()) return; // if true return
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        
        bool placementValidity = CheckPlacementValidity(cellPos, selectedObjectIndex);
        if(placementValidity == false)
            return;

        GameObject placeableItem = Instantiate(dataBase.objsData[selectedObjectIndex].prefab); //fetching prfab according to the objs id
        placeableItem.transform.position = grid.CellToWorld(cellPos);
        placedGameObjs.Add(placeableItem);
        GridData selectedData = plantsData;
        selectedData.AddObjectAt(cellPos,
            dataBase.objsData[selectedObjectIndex].Size,
            dataBase.objsData[selectedObjectIndex].ID,
            placedGameObjs.Count - 1);
        preview.UpdatePosition(grid.CellToWorld(cellPos), false);     
    }

    private bool CheckPlacementValidity(Vector3Int cellPos, int selectedObjectIndex)
    {
        GridData selectedData = plantsData;
        return selectedData.CanPlaceObjectAt(cellPos, dataBase.objsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceObjects;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPos = Vector3Int.zero;
    }

    private void Update()
    {
        if(selectedObjectIndex < 0) return;
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        
        if(lastDetectedPos != cellPos)
        {
            bool placementValidity = CheckPlacementValidity(cellPos, selectedObjectIndex);
        
            objectIndicator.transform.position = touchPos;
            preview.UpdatePosition(grid.CellToWorld(cellPos), placementValidity);
            lastDetectedPos = cellPos;
    
        }
        
    }
}
