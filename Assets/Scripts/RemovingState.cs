using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSys previewSys;
    GridData plantData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid, // how does this work??
                         PreviewSys previewSys,
                         GridData plantdata,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSys = previewSys;
        this.plantData = plantdata; // is the script = griddata = plantdata??? 
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
        if(plantData.CanPlaceObjectAt(cellPos,Vector2Int.one) == false) // check if there is obj on the cell to remove
        {
            selectedData = plantData;
        }
        if(selectedData == null) // if no obj play nope sound
        {
            //sound
        }
        else // Remove Obj
        {
            gameObjectIndex = selectedData.GetRepresnetationIndex(cellPos);
            if(gameObjectIndex == -1) return;
            selectedData.RemoveObjectAt(cellPos); // removes index from list grid in short, i guess    
            objectPlacer.RemoveObjectAt(gameObjectIndex); // removes the obj itself

        }
        Vector3 cellPosition = grid.CellToWorld(cellPos);
        previewSys.UpdatePosition(cellPosition,CheckIfSelectionIsValid(cellPos)); 
    }

    //public void RemoveObjs(Vector3Int cellPos) // remove this function later
    //{
    //    GridData selectedData = null;
    //    gameObjectIndex = selectedData.GetRepresnetationIndex(cellPos); // getting cell of the placed obj
    //    if(gameObjectIndex == -1) return; // if cell is empty return
    //    selectedData.RemoveObjectAt(cellPos);  // else make cell empty again in griddata
    //    objectPlacer.RemoveObjectAt(gameObjectIndex); // remove game obj at cell pos
    //    Vector3 cellPosition = grid.CellToWorld(cellPos); // update cell pos
    //    previewSys.UpdatePosition(cellPosition,CheckIfSelectionIsValid(cellPos)); // update preview sys 
    //}

    private bool CheckIfSelectionIsValid(Vector3Int cellPos)
    {
        return !(plantData.CanPlaceObjectAt(cellPos,Vector2Int.one));
    }

    public void UpdateState(Vector3Int cellPos) // updating grid data if cell is occupied or not
    {
        bool validity = CheckIfSelectionIsValid(cellPos);
        previewSys.UpdatePosition(grid.CellToWorld(cellPos), validity);
    }
}
