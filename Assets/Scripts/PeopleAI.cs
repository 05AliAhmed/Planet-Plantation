using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonStateType { Roaming, TreeInteraction, Returning, Resting }

// Roaming -> (tree in range) -> TreeInteraction -> Returning -> Resting -> Roaming ...
// Each state method starts one coroutine; a state ends by calling the next state's method.
public class PeopleAI : MonoBehaviour, IPeopleState
{
    [Header("Debug (read only)")]
    [SerializeField] private PersonStateType currentState;
    [SerializeField] private Vector3Int homeCell;
    [SerializeField] private Vector3Int lastCell;
    [SerializeField] private TreeBehaviour treeInAffect;

    private PeopleWorld world;
    private PeopleSettingSO cfg;
    private int personId = -1;
    private bool initialized;

    // cells this person currently holds in the people GridData
    private Vector3Int currentCell;
    private bool hasNextCell;
    private Vector3Int nextCell;

    // destination reservation (so two people never head for the same cell)
    private bool hasReservation;
    private Vector3Int reservedCell;

    private Coroutine running;
    private bool lastMoveOk;
    private int roamsDone;

    private const int MaxTreeAttemptsPerCycle = 3;

    // Called by PeopleSpawner right after Instantiate. The spawner has already
    // claimed spawnCell in the people GridData for this id.
    public void Initialize(int id, Vector3Int spawnCell)
    {
        world = PeopleWorld.Instance;
        cfg = world.Settings;
        personId = id;
        homeCell = lastCell = currentCell = spawnCell;
        transform.position = world.CellCenter(spawnCell);
        initialized = true;
        Roaming();
    }

    private void Start()
    {
        if (!initialized)
            Debug.LogWarning("PeopleAI: this person wasn't spawned by PeopleSpawner, so it won't move.", this);
    }

    private void OnDestroy()
    {
        if (!initialized || world == null) return;
        world.PeopleGrid.Vacate(currentCell, personId);
        if (hasNextCell) world.PeopleGrid.Vacate(nextCell, personId);
        world.ReleaseAllReservations(personId);
        if (treeInAffect != null) treeInAffect.ReleaseClaim(this, 0f);
    }

    // ======================= IPeopleState =======================

    public void Roaming() { EnterState(PersonStateType.Roaming, RoamingRoutine()); }
    public void TreeInteraction() { EnterState(PersonStateType.TreeInteraction, TreeInteractionRoutine()); }
    public void Returning() { EnterState(PersonStateType.Returning, ReturningRoutine()); }
    public void Resting() { EnterState(PersonStateType.Resting, RestingRoutine()); }

    private void EnterState(PersonStateType state, IEnumerator routine)
    {
        currentState = state;
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(routine);
    }

    // ======================= 1. ROAMING =======================

    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            // --- tree in the ranged area around lastCell? go to the closest one ---
            int treeAttempts = 0;
            foreach (TreeBehaviour tree in GetTreesInRange(lastCell))
            {
                if (treeAttempts++ >= MaxTreeAttemptsPerCycle) break;
                if (tree == null || !tree.TryClaim(this)) continue;

                treeInAffect = tree;
                // stand on any free cell touching the tree (radius 1)
                yield return MoveToward(world.WorldToCell(tree.transform.position), 1, true);
                lastCell = currentCell;

                if (lastMoveOk && tree != null)
                {
                    TreeInteraction();      // arrived next to the tree
                    yield break;
                }

                // couldn't reach it (blocked, or the tree was destroyed): drop it
                if (tree != null) tree.ReleaseClaim(this, 0f);
                treeInAffect = null;
            }

            // --- otherwise pick a random free cell in the area and walk there ---
            for (int attempt = 0; attempt < 5; attempt++)
            {
                if (!TryPickRandomCell(out Vector3Int target)) break;
                yield return MoveToward(target, 0, false);
                lastCell = currentCell;
                if (lastMoveOk) break;
            }

            roamsDone++;
            yield return new WaitForSeconds(Random.Range(cfg.waitMin, Mathf.Max(cfg.waitMin, cfg.waitMax)));

            // optional: after N roams, go home and rest
            if (cfg.maxRoamsBeforeRest > 0 && roamsDone >= cfg.maxRoamsBeforeRest)
            {
                Returning();
                yield break;
            }
        }
    }

    // Trees inside the square area around 'centre', closest first, only ones people may use.
    private List<TreeBehaviour> GetTreesInRange(Vector3Int centre)
    {
        var result = new List<TreeBehaviour>();
        if (NatureManager.Instance == null) return result;

        int r = cfg.roamRadius;
        foreach (TreeBehaviour t in NatureManager.Instance.trees)
        {
            if (t == null || !t.IsAvailableForPeople) continue;
            Vector3Int c = world.WorldToCell(t.transform.position);
            if (Mathf.Abs(c.x - centre.x) > r || Mathf.Abs(c.z - centre.z) > r) continue;
            if (!world.TreeGrid.IsOccupied(c)) continue;   // not actually placed on the grid (e.g. preview ghost)
            result.Add(t);
        }

        result.Sort((a, b) =>
            (world.WorldToCell(a.transform.position) - centre).sqrMagnitude
            .CompareTo((world.WorldToCell(b.transform.position) - centre).sqrMagnitude));
        return result;
    }

    private bool TryPickRandomCell(out Vector3Int cell)
    {
        int r = cfg.roamRadius;
        var candidates = new List<Vector3Int>();
        for (int x = lastCell.x - r; x <= lastCell.x + r; x++)
        {
            for (int z = lastCell.z - r; z <= lastCell.z + r; z++)
            {
                var c = new Vector3Int(x, 0, z);
                if (c == currentCell) continue;
                if (world.IsWalkable(c, personId)) candidates.Add(c);
            }
        }

        if (candidates.Count == 0) { cell = currentCell; return false; }
        cell = candidates[Random.Range(0, candidates.Count)];
        return true;
    }

    // ======================= 2. TREE INTERACTION =======================

    private IEnumerator TreeInteractionRoutine()
    {
        TreeBehaviour tree = treeInAffect;
        if (tree == null)
        {
            Roaming();
            yield break;
        }

        FaceTowards(tree.transform.position);
        tree.TriggerEvent(Random.Range(0, 3), gameObject);   // 0 fire, 1 drought, 2 storm

        // let the event play out (stop early if NatureManager removes the tree meanwhile)
        float elapsed = 0f;
        while (tree != null && elapsed < tree.PersonEventDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (tree != null) tree.ReleaseClaim(this, cfg.treeCooldown);
        treeInAffect = null;
        Returning();
    }

    // ======================= 3. RETURNING =======================

    private IEnumerator ReturningRoutine()
    {
        // go to the home cell; if someone is standing on it, settle on the nearest free cell
        for (int radius = 0; radius <= cfg.homeSearchRadius; radius++)
        {
            yield return MoveToward(homeCell, radius, false);
            lastCell = currentCell;
            if (lastMoveOk) break;
        }
        homeCell = currentCell;     // home is wherever we ended up (normally the original cell)
        Resting();
    }

    // ======================= 4. RESTING =======================

    private IEnumerator RestingRoutine()
    {
        roamsDone = 0;
        yield return new WaitForSeconds(cfg.restTime);
        Roaming();
    }

    // ======================= MOVEMENT =======================

    // Walks along an A* path to any walkable cell within goalRadius of goalCenter.
    // Sets lastMoveOk. If watchTree is true, aborts when treeInAffect is destroyed.
    private IEnumerator MoveToward(Vector3Int goalCenter, int goalRadius, bool watchTree)
    {
        lastMoveOk = false;
        TreeBehaviour tree = treeInAffect;

        List<Vector3Int> path = PlanPath(goalCenter, goalRadius);
        if (path == null) yield break;

        int repaths = 0;
        int i = 0;
        try
        {
            while (i < path.Count)
            {
                if (watchTree && tree == null) yield break;     // target tree was destroyed

                Vector3Int next = path[i];

                // blocked if a tree appeared, someone reserved it, or someone stepped in
                bool blocked = world.TreeGrid.IsOccupied(next)
                               || world.IsReservedByOther(next, personId)
                               || !world.PeopleGrid.TryOccupy(next, personId);

                if (blocked)
                {
                    if (++repaths > cfg.maxRepaths) yield break;
                    yield return new WaitForSeconds(cfg.blockedRetryDelay);
                    path = PlanPath(goalCenter, goalRadius);
                    if (path == null) yield break;
                    i = 0;
                    continue;
                }

                nextCell = next;
                hasNextCell = true;
                Vector3 target = world.CellCenter(next);
                FaceTowards(target);

                while ((transform.position - target).sqrMagnitude > 0.0001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target,
                                                             cfg.moveSpeed * Time.deltaTime);
                    yield return null;
                }

                world.PeopleGrid.Vacate(currentCell, personId);
                currentCell = next;
                hasNextCell = false;
                i++;
            }
            lastMoveOk = true;
        }
        finally
        {
            ReleaseReservation();
        }
    }

    private List<Vector3Int> PlanPath(Vector3Int goalCenter, int goalRadius)
    {
        ReleaseReservation();
        List<Vector3Int> path = world.FindPath(currentCell, goalCenter, goalRadius, personId);
        if (path != null && path.Count > 0)
        {
            reservedCell = path[path.Count - 1];
            hasReservation = true;
            world.Reserve(reservedCell, personId);
        }
        return path;
    }

    private void ReleaseReservation()
    {
        if (!hasReservation) return;
        world.ReleaseReservation(reservedCell, personId);
        hasReservation = false;
    }

    private void FaceTowards(Vector3 worldPos)
    {
        Vector3 dir = worldPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir);
    }
}