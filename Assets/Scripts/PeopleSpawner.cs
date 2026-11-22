using UnityEngine;

// Spawns people on random free cells of the ground.
// A cell is "free" if it has no tree (tree GridData) and no person (people GridData).
public class PeopleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject personPrefab;   // must have a PeopleAI component
    [SerializeField] private Transform peopleParent;    // optional, keeps the hierarchy tidy
    [SerializeField] private bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart) SpawnAll();
    }

    public void SpawnAll()
    {
        int count = PeopleWorld.Instance.Settings.spawnCount;
        for (int i = 0; i < count; i++) SpawnOne();
    }

    public bool SpawnOne()
    {
        PeopleWorld world = PeopleWorld.Instance;
        int attempts = world.Settings.spawnAttempts;

        for (int a = 0; a < attempts; a++)
        {
            Vector3Int cell = world.RandomCellInBounds();
            if (world.TreeGrid.IsOccupied(cell)) continue;      // tree here, pick a new cell

            int id = world.NextPersonId();
            if (!world.PeopleGrid.TryOccupy(cell, id)) continue; // person here, pick a new cell

            GameObject go = Instantiate(personPrefab, world.CellCenter(cell), Quaternion.identity, peopleParent);
            PeopleAI ai = go.GetComponent<PeopleAI>();
            if (ai == null)
            {
                Debug.LogError("PeopleSpawner: person prefab has no PeopleAI component.", this);
                world.PeopleGrid.Vacate(cell, id);
                Destroy(go);
                return false;
            }
            ai.Initialize(id, cell);
            return true;
        }

        Debug.LogWarning("PeopleSpawner: couldn't find a free cell to spawn a person.", this);
        return false;
    }
}
