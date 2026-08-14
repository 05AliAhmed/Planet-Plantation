using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class NatureManager : MonoBehaviour
{
    public static NatureManager Instance;

    [SerializeField] private Transform planeD;
    [SerializeField] private Grid grid;
    [SerializeField] private GridData plantsData;
    [SerializeField] private ObjectPlacer objectPlacer;  
    [SerializeField] private PreviewSys preview;
    private RemovingState removingState;
    private IBuildingState buildingState;
    private PlacementSys placementSys;
    Renderer rend;
    public int randEventNum;
    public float waitTime;
    public List<TreeBehaviour> trees = new List<TreeBehaviour>(); // can also be written like public List<TreeBehaviour> trees = new();
    public List<TreeBehaviour> closestTrees;
    private void Awake()
    {
        Instance = this;
        StartCoroutine(RandomTreeEventLoop());
    }

    private void Start()
    {
        rend = planeD.GetComponent<Renderer>();
        placementSys = GameObject.Find("PlacementSys").GetComponent<PlacementSys>();
        SetRemovingState();

    }

    public void SetRemovingState()
    {
        // buildingState = new RemovingState(grid,
        //                                 preview,
        //                                 plantsData,
        //                                 objectPlacer);
        removingState = new RemovingState(grid,
                                                preview,
                                                plantsData,
                                                objectPlacer);
        // buildingState = new RemovingState()
    }
    public void AddTree(TreeBehaviour tree)
    {
        trees.Add(tree);
        // Update/Indexes();
    }

    public void RemoveTree(TreeBehaviour tree)
    {
        trees.Remove(tree);
        // UpdateIndexes();
    }

    // void UpdateIndexes() // giving indexes to the trees in the main list
    // {
    //     for (int i = 0; i < trees.Count; i++)
    //     {
    //         trees[i].SetIndex(i);
    //     }
    // }

    IEnumerator RandomTreeEventLoop() // waittime for event loops
    {
        while (true)
        {
            float waitTime = Random.Range(29f, 30f);
            yield return new WaitForSeconds(waitTime);
            // Debug.Log(waitTime);
            Debug.Log(GetRandomPoint());
            TriggerEventOnClosestTrees();
        }
    }

    // void TriggerRemove(Vector3Int cellPos)
    // {
    //     IBuildingState state = placementSys.GetCurrentState();
    //     Debug.Log(state);
    //     RemovingState removingState = state as RemovingState;

    //     if (removingState != null)
    //     {
    //         removingState.RemoveObjs(cellPos);
    //     }
    // }

    void TriggerEventOnClosestTrees()
    {
        randEventNum = Random.Range(0, 3);
        Vector3 randomPoint = GetRandomPoint(); // getting random point on plane

        closestTrees = trees  // list of closeby trees
            .OrderBy(tree => Vector3.Distance(tree.transform.position, randomPoint)) // trees close of random point
            .Take(6) // number of trees to be effected
            .ToList(); // adding to the list

        foreach (var tree in closestTrees) // for each tree in closetree  list triggerevnets from trebehaviour
        {
            tree.TriggerEvent(randEventNum); // every tree is playing rnd 
            // startcoroutine(waitTime)
            Vector3 treePos = tree.transform.position;
            Vector3Int cellPos = grid.WorldToCell(treePos);
            removingState.RemoveObjs(cellPos);
            // TriggerRemove(cellPos);
        }
    }

    IEnumerator WaitTBeforeAE() // waittimeBeforeAfterEvent
    {
        yield return new WaitForSeconds(waitTime);

        // Destroy(gameObject);
        // objectPlacer.RemoveObjectAt(RemovingState.gameObjectIndex);
    }
    Vector3 GetRandomPoint()
    {
        Vector3 size = rend.bounds.size; // taking total size of the plane using renderer
        Vector3 center = rend.bounds.center; // accessing center point using bounds
        float minX = center.x - size.x/2; // min and max
        float maxX = center.x + size.x/2;
        float minz = center.z - size.z/2;
        float maxz = center.z + size.z/2; 
        float x = Random.Range(0f, 10f); // getting rnd point on x 
        float z = Random.Range(0f, 10f); // getting rnd point on y 

        return new Vector3(x, 0, z); // cordinate of rnd point on the plane
    }

    
}
