using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjs = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject placeableItem = Instantiate(prefab); //fetching prfab according to the objs id
        placeableItem.transform.position = position;
        placedGameObjs.Add(placeableItem);
        return placedGameObjs.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjs.Count <= gameObjectIndex || placedGameObjs[gameObjectIndex] == null) return;
        Destroy(placedGameObjs[gameObjectIndex]);
        placedGameObjs[gameObjectIndex] = null;
    }
}
