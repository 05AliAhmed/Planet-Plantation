using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class NatureManager : MonoBehaviour
{
    public static NatureManager Instance;

    [SerializeField] Transform planeD;
    Renderer rend;
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

    }
    public void AddTree(TreeBehaviour tree)
    {
        trees.Add(tree);
        UpdateIndexes();
    }

    public void RemoveTree(TreeBehaviour tree)
    {
        trees.Remove(tree);
        UpdateIndexes();
    }

    void UpdateIndexes() // giving indexes to the trees in the main list
    {
        for (int i = 0; i < trees.Count; i++)
        {
            trees[i].SetIndex(i);
        }
    }

    IEnumerator RandomTreeEventLoop() // waittime for event loops
    {
        while (true)
        {
            float waitTime = Random.Range(20f, 30f);
            yield return new WaitForSeconds(waitTime);
            Debug.Log(waitTime);
            Debug.Log(GetRandomPoint());
            TriggerEventOnClosestTrees();
        }
    }

    void TriggerEventOnClosestTrees()
    {
        Vector3 randomPoint = GetRandomPoint(); // getting random point on plane

        closestTrees = trees  // list of closeby trees
            .OrderBy(tree => Vector3.Distance(tree.transform.position, randomPoint)) // trees close of random point
            .Take(6) // number of trees to be effected
            .ToList(); // adding to the list

        foreach (var tree in closestTrees) // for each tree in closetree  list triggerevnets from trebehaviour
        {
            tree.TriggerEvent();
            // Debug.Log(GetRandomPoint());
        }
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
