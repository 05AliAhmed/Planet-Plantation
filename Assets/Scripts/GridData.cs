using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    public void AddObjectAt(Vector3Int gridPos, Vector2Int objSize, int ID, int placedObjectIndex){
        
        List<Vector3Int> posToOccupy = CalculatePos(gridPos, objSize);
        PlacementData data = new PlacementData(posToOccupy, ID, placedObjectIndex);
        foreach(var pos in posToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary alreaft contains this cell pos {pos}");
            placedObjects[pos] = data;
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
}
public class PlacementData
{
    public List<Vector3Int> occupiedPos;
    public int ID{get; private set;}
    public int placedObjIndex{get; private set;}

    public PlacementData(List<Vector3Int> occupiedPos, int id, int placedobjectsindex)
    {
        this.occupiedPos = occupiedPos;
        ID = id;
        placedObjIndex = placedobjectsindex;
    }
}