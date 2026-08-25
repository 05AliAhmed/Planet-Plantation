using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlacementSys : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjDatabaseSO dataBase;
    [SerializeField] private GameObject gridVisualization; public GridData plantsData;
    [SerializeField] private ObjectPlacer objectPlacer;  
    [SerializeField] private PreviewSys preview; private Vector3Int lastDetectedPos = Vector3Int.zero;
    IBuildingState buildingState;
    // UIManager uIManager;

    private void Awake()
    {
        plantsData = new GridData();
    }

    private void Start()
    {
        // uIManager = FindAnyObjectByType<UIManager>();
        // uIManager = GameObject.Find("UImanager").GetComponent<UIManager>();
        StopPlacement();
        //plantsData = new();
    }

    public GridData GetGridData() // THIS IS THE GETTER ## NOT WORKING THOUGH
    {
        return plantsData;
    }

    public void StartPlacement(int ID){
        StopPlacement();
        inputManager.isPlacing = true;
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

    public void StartRemoving() // call strtremoving in trigger events that will chane the building state 
    {
        StopPlacement();
        gridVisualization.SetActive(false);
        buildingState = new RemovingState(grid,
                                          preview,
                                          plantsData,
                                          objectPlacer);
        // NatureManager.Instance.SetRemovingState();
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
    public IBuildingState GetCurrentState()
    {
        return buildingState;
    }

    public void StopPlacement()
    {
        gridVisualization.SetActive(false);
        inputManager.isPlacing = false;
        // uIManager.scrollMenu.SetActive(false);
        if(buildingState == null) return;
        buildingState.EndState();
        inputManager.OnClicked -= PlaceObjects;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPos = Vector3Int.zero;
        // buildingState = null;
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
