using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class NatureManager : MonoBehaviour
{
    public static NatureManager Instance;

    [SerializeField] private Transform planeD;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectPlacer objectPlacer;
    private GridData plantsGrid; // needs reference in runtime, but how?? is the thing. Can attach it to empty gameobj in outliner and ref it... NOT WORKING 
    //[SerializeField] private ObjectPlacer objectPlacer;  
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
        plantsGrid = placementSys.GetGridData(); // 18. new way chat

        Debug.Log("PlacementSys: " + placementSys);
        Debug.Log("PlantsGrid: " + plantsGrid);

    }

    //public void SetRemovingState()
    //{
    //    // buildingState = new RemovingState(grid,
    //    //                                 preview,
    //    //                                 plantsData,
    //    //                                 objectPlacer);
    //    //removingState = new RemovingState(grid,
    //    //                                        preview,
    //    //                                        plantsData,
    //    //                                        objectPlacer);
    //    // buildingState = new RemovingState()
    //}
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

    IEnumerator RandomTreeEventLoop() // waittime for event loops
    {
        while (true)
        {
            float waitTime = Random.Range(29f, 30f);
            yield return new WaitForSeconds(waitTime);
            // Debug.Log(waitTime);
            Debug.Log(GetRandomPoint());
            if (!GameState.IsPaused) TriggerEventOnClosestTrees();
        }
    }

    // TRIED SOLVING AN ISSUE BY MYSELF WORDKED BUT STILL HAD LOOPHOLES
    //void TriggerEventOnClosestTrees()
    //{
    //    randEventNum = Random.Range(0, 3);
    //    Vector3 randomPoint = GetRandomPoint(); // getting random point on plane

    //    closestTrees = trees  // list of closeby trees
    //        .OrderBy(tree => Vector3.Distance(tree.transform.position, randomPoint)) // trees close of random point
    //        .Take(6) // number of trees to be effected
    //        .ToList(); // adding to the list

    //    foreach (var tree in closestTrees) // for each tree in closetree  list triggerevnets from trebehaviour
    //    {
    //        tree.TriggerEvent(randEventNum); // every tree is playing rnd 

    //        StartCoroutine(WaitTBeforeAE()); // will this just wait for time and then return out to play the rest of the code or do i need to put all of the below stuff in waitbeforeafer function

    //        Vector3 treePos = tree.transform.position; // getting position of current tree in the closest list | .1
    //        Vector3Int cellPos = grid.WorldToCell(treePos); // assigning it on the grid as cellpos | POS CALLED N ASSIGNED .1
    //        //int gameObjectIndex = plantsData.GetRepresnetationIndex(cellPos);
    //        //if (gameObjectIndex == -1) return;
    //        //plantsData.RemoveObjectAt(cellPos); // removes index from list grid in short, i guess    
    //        //objectPlacer.RemoveObjectAt(gameObjectIndex); // removes the obj itself // WILL STILL NEED TO CALL THIS TO REMOVE TREE FROM DICTIONARY(LIST), OPTIMIZATION?? REASONS
    //        // 1. HOW DO I CALL GRID POS HERE # DONE
    //        // 2. HOW DO I REMOVE IT FROM THE OCCUPANCY ON THE GRID # TRYING TO CALL PLANTDATA MAKING GRIDDATA SCRPT AN INSTANCE
    //        // 4. NULL REF ERROR LINE 101. MAYBE INSTANCING DIDNT WORK # LETS TRY DOING OTHER STUFF IN GRID DATA SCRPT REMOVING OBJS MANUALY ## WORKING JUST FINE MEANS INSTANCING NOT AN ISSUE??
    //        // 5. DISPLAYING CEll POS TO SEE IF ITS BEING ACCESSED ## WORKING
    //        // 6. IS IT BECAUSE OF THE COROUTINE?? NOPE I GUESS ERROR IN TREE BEHAVE SCPT 7. ... in tree behaviour
    //        // 9. LETS HOPE IT WORKS NOW ## NOPW STILL GIVING THE ERROR IGN 7. 8. n 9.
    //        // 10. ...in griddata ## NOT WORKING STILL
    //        // 11. MAYBE ITS A REFERENCING ISSUE LESS TRY DEBUGING BUT IN GRID DATA AND CALL IT HERE TO CHECK.
    //        // 13. I AM DUMB F*** PROBLEM "IT SHOULD BE THE PROBLEM NOW", IS I MADE STATIC GRID DATA abc BUT DIDN'T ASSIGN abc TO THAT SCRIPT > "abc = this;" THIS THING AAAAAAAAHHHHHHHHHHH!!!!!!!!!!! SHOULD WORK NNOW
    //        // 14. ASSIGNED GRIDDATA IN AWKE IN THIS SCPT NOT WORKING GRIDDATA WHY NO IDEA
    //        // WILL THIS WORK???? SUS USED CHAT CREATED A GETTER IN PLACEMENTSYS AS GRID DATA IS ALREADY ASSIGNED THERE AND CALLED IT HERE
    //        // 16. plantsData = new GridData(); ADDED THIS IN START TO COMPLETE REFERENCING OF GRID DATA ## reason for new error, creating two griddatas now das why error
    //        // 17. less try new way less do placementsys.plantsdata and less assign it to GridData -> plantsGrid will this work
    //        // 18. Finally working baby bbut new issue :) assigned plantsdata = new() is awake in placement sys. YEESSSSSSSSsSSSSSSS !!!!!!!!!!! AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //        // ISSUE NOW IS IT IS NOW WAITING FOR 5 SEC BEFORE DESTROYING BUT HAPPY FOR TODAY REST LATER :| 
    //        //Debug.Log(plantsGrid.Debugging()); // 12.NOT WORKING IT IS REFERENECING ISSUE B/C PLACMENTDATA METHOD NOT CALLED IN HERe
    //        //Debug.Log(cellPos);
    //        plantsGrid.GetRepresnetationIndex(cellPos); // #### LESS HOPE!!!!! IT WILL WORK NOW
    //        plantsGrid.RemoveObjectAt(cellPos); // 3.LEARNED A NEW THING .INSTANCE(HERE.abc) is just a name to var type script >> public static SCRIPT VAR; WOOOOahww. 
    //        RemoveTree(tree); // gameojb removed from the list closest tree
    //        Destroy(tree.gameObject); // gameobject destroyed
    //    }
    //}

    // NOW USING CLAUDE, SAME FUNTION AS ABOVE

    void TriggerEventOnClosestTrees()
    {
        randEventNum = Random.Range(0, 3);
        Vector3 randomPoint = GetRandomPoint();

        closestTrees = trees
            .Where(t => t != null)
            .OrderBy(tree => Vector3.Distance(tree.transform.position, randomPoint))
            .Take(6)
            .ToList();

        foreach (var tree in closestTrees)
        {
            tree.TriggerEvent(randEventNum);                 // play the event animation
            StartCoroutine(RemoveTreeAfter(tree, waitTime)); // remove it after waitTime seconds
        }
    }

    IEnumerator RemoveTreeAfter(TreeBehaviour tree, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tree == null) yield break;                       // a person already removed it

        Vector3Int cellPos = grid.WorldToCell(tree.transform.position);
        int index = plantsGrid.GetRepresnetationIndex(cellPos);
        if (index == -1) yield break;                        // not on the grid (e.g. preview ghost), leave it

        plantsGrid.RemoveObjectAt(cellPos);                  // frees the cell in the data
        objectPlacer.RemoveObjectAt(index);                  // destroys the tree object
    }

    //IEnumerator WaitTBeforeAE() // waittimeBeforeAfterEvent
    //{
    //    yield return new WaitForSeconds(waitTime);
    //}
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
