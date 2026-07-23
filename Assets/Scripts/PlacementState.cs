using Unity.Android.Gradle.Manifest;
using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int cellPos);
    void UpdateState(Vector3Int cellPos);
}

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSys previewSys;
    ObjDatabaseSO dataBase;
    GridData plantData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSys previewSys,
                          ObjDatabaseSO dataBase,
                          GridData plantdata,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSys = previewSys;
        this.dataBase = dataBase;
        this.plantData = plantdata;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = dataBase.objsData.FindIndex(objInd => objInd.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSys.StartShowingPlacementPreview(dataBase.objsData[selectedObjectIndex].prefab,
                                            dataBase.objsData[selectedObjectIndex].Size);
        }
        else throw new System.Exception($"No Object With ID {iD}");

    }

    public void EndState()
    {
        previewSys.StopShowingPreview();
    }

    public void OnAction(Vector3Int cellPos)
    {
        bool placementValidity = CheckPlacementValidity(cellPos, selectedObjectIndex);
        if (placementValidity == false)
            return;

        int index = objectPlacer.PlaceObject(dataBase.objsData[selectedObjectIndex].prefab, grid.CellToWorld(cellPos));

        GridData selectedData = plantData;
        selectedData.AddObjectAt(cellPos,
            dataBase.objsData[selectedObjectIndex].Size,
            dataBase.objsData[selectedObjectIndex].ID,
            index);
        previewSys.UpdatePosition(grid.CellToWorld(cellPos), false);
    }

    private bool CheckPlacementValidity(Vector3Int cellPos, int selectedObjectIndex)
    {
        GridData selectedData = plantData;
        return selectedData.CanPlaceObjectAt(cellPos, dataBase.objsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int cellPos)
    {
        bool placementValidity = CheckPlacementValidity(cellPos, selectedObjectIndex);
        previewSys.UpdatePosition(grid.CellToWorld(cellPos), placementValidity);
    }
}
