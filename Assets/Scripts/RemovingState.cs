using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSys previewSys;
    GridData plantData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSys previewSys,
                         GridData plantdata,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSys = previewSys;
        this.plantData = plantdata;
        this.objectPlacer = objectPlacer;

        previewSys.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSys.StopShowingPreview();
    }

    public void OnAction(Vector3Int cellPos)
    {
        GridData selectedData = null;
        if(plantData.CanPlaceObjectAt(cellPos,Vector2Int.one) == false)
        {
            selectedData = plantData;
        }
        if(selectedData == null)
        {
            //sound
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresnetationIndex(cellPos);
            if(gameObjectIndex == -1) return;
            selectedData.RemoveObjectAt(cellPos);     
            objectPlacer.RemoveObjectAt(gameObjectIndex);

        }
        Vector3 cellPosition = grid.CellToWorld(cellPos);
        previewSys.UpdatePosition(cellPosition,CheckIfSelectionIsValid(cellPos)); 
    }

    private bool CheckIfSelectionIsValid(Vector3Int cellPos)
    {
        return !(plantData.CanPlaceObjectAt(cellPos,Vector2Int.one));
    }

    public void UpdateState(Vector3Int cellPos)
    {
        bool validity = CheckIfSelectionIsValid(cellPos);
        previewSys.UpdatePosition(grid.CellToWorld(cellPos), validity);
    }
}
