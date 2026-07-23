using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlacementSys : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField]
    private ObjDatabaseSO dataBase;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData plantsData;
    [SerializeField]   
    private ObjectPlacer objectPlacer;  

    [SerializeField]
    private PreviewSys preview;

    private Vector3Int lastDetectedPos = Vector3Int.zero;

    IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        plantsData = new();
    }

    public void StartPlacement(int ID){
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           dataBase,
                                           plantsData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceObjects;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(false);
        buildingState = new RemovingState(grid,
                                          preview,
                                          plantsData,
                                          objectPlacer);
        inputManager.OnClicked += PlaceObjects;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceObjects() // if can be added to confirm where to place the object
    {
        if(inputManager.IsPointerOverUI()) return; // if true return
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);

        buildingState.OnAction(cellPos);    
    }

    // private bool CheckPlacementValidity(Vector3Int cellPos, int selectedObjectIndex)
    // {
    //     GridData selectedData = plantsData;
    //     return selectedData.CanPlaceObjectAt(cellPos, dataBase.objsData[selectedObjectIndex].Size);
    // }

    private void StopPlacement()
    {
        if(buildingState == null) return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceObjects;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPos = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if(buildingState == null) return;
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        
        if(lastDetectedPos != cellPos)
        {
            buildingState.UpdateState(cellPos);
            lastDetectedPos = cellPos;
    
        }
        
    }
}
