using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridData 
{

    //public static GridData abc;
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    void start()
    {

    }
    public void AddObjectAt(Vector3Int gridPos, Vector2Int objSize, int ID, int placedObjectIndex){ // OBJS BEING ADDED TO THE LIST POS ASSINGED TO "POS"
        
        List<Vector3Int> posToOccupy = CalculatePos(gridPos, objSize);
        PlacementData data = new PlacementData(posToOccupy, ID, placedObjectIndex);
        foreach(var pos in posToOccupy)
        {
            if(placedObjects.ContainsKey(pos)) // SAFE CHECK
                throw new Exception($"Dictionary alreaft contains this cell pos {pos}");
            placedObjects[pos] = data; // ELSE RETURN THE DATA ON THE CELL
        }
    }

    private List<Vector3Int> CalculatePos(Vector3Int gridPos, Vector2Int objSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objSize.x; x++)
        {
            for (int y = 0; y < objSize.y; y++)
            {
                returnVal.Add(gridPos + new Vector3Int(x,0,y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPos, Vector2Int objSize)
    {
        List<Vector3Int> posToOccupy = CalculatePos(gridPos, objSize);
        foreach(var pos in posToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
                return false;
        }
        return true;

    }

    public int GetRepresnetationIndex(Vector3Int cellPos) // this connected with remove obj? // THIS RETURN THE INDEX OG THE OBJ IN THE LIST
    {
        if(placedObjects.ContainsKey(cellPos) ==false) return -1;
        return placedObjects[cellPos].placedObjIndex;
    }

    public void RemoveObjectAt(Vector3Int cellPos) // 10. NEW GUESS N.M ALSO NEEDS GETREPINDX METHOD B/C IT WORKS WITH REMOBJ METHOD. AND THIS REMOVES THE OBJ IN THE LIST OCCUPIED
                                                   // POS WITHIN THE DICTIONARY HOPING THIS IS THE CASE OR IDK WHAT IT IS 
    {
        //Debug.Log("Inside griddata removeobjat()"); // now error is here means reference thorugh?? but cannot find the key 
        foreach(var pos in placedObjects[cellPos].occupiedPos)
        {
            placedObjects.Remove(pos);
        }
    }

    public int Debugging() // 11.TESTING IN NATURE MANAGER
    {
        int abd = 2034;
        return abd;
    }



}
public class PlacementData
{
    public List<Vector3Int> occupiedPos;
    public int ID{get; private set;}
    public int placedObjIndex{get; private set;}

    public PlacementData(List<Vector3Int> occupiedPos, int id, int placedobjectsindex) // THIS IS GRID DATA LIST // 12. CAN THIS BE  PROB, CHECK NEXT
    {
        this.occupiedPos = occupiedPos;
        ID = id;
        placedObjIndex = placedobjectsindex;
    }
}
