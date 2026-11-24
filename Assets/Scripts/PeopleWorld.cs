using System.Collections.Generic;
using UnityEngine;

// Shared services for all people: grid conversion, ground bounds, the people
// GridData, walkability, destination reservations and pathfinding.
// Put ONE of these in the scene (e.g. on an empty "PeopleSystem" object).
public class PeopleWorld : MonoBehaviour
{
    public static PeopleWorld Instance { get; private set; }

    [SerializeField] private Grid grid;
    [SerializeField] private Renderer ground;            // renderer of the ground plane people walk on
    [SerializeField] private PlacementSys placementSys;  // gives us the tree GridData
    [SerializeField] private PeopleSettingSO settings;
    [SerializeField] private ObjectPlacer objectPlacer;
    public ObjectPlacer ObjectPlacer => objectPlacer;
    public PeopleSettingSO Settings => settings;

    // Separate GridData for people so they never block tree placement.
    public GridData PeopleGrid { get; } = new GridData();

    // Trees live in PlacementSys's GridData. Fetched lazily because
    // PlacementSys creates it in its own Awake (order between Awakes isn't guaranteed).
    private GridData treeGrid;
    public GridData TreeGrid
    {
        get
        {
            if (treeGrid == null && placementSys != null) treeGrid = placementSys.GetGridData();
            return treeGrid;
        }
    }

    public Vector3Int MinCell { get; private set; }
    public Vector3Int MaxCell { get; private set; }

    private float groundTopY;
    private int nextPersonId;
    private readonly Dictionary<Vector3Int, int> reserved = new Dictionary<Vector3Int, int>();

    private void Awake()
    {
        Instance = this;
        if (grid == null || ground == null || placementSys == null || settings == null)
        {
            Debug.LogError("PeopleWorld: assign Grid, Ground, PlacementSys and Settings in the Inspector.", this);
            return;
        }
        RecalculateBounds();
    }

    public void RecalculateBounds()
    {
        Bounds b = ground.bounds;
        MinCell = WorldToCell(b.min);
        MaxCell = WorldToCell(new Vector3(b.max.x - 0.001f, b.center.y, b.max.z - 0.001f));
        groundTopY = b.max.y;
    }

    // ---------- conversions (cells always have y = 0, like GridData expects) ----------
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        Vector3Int c = grid.WorldToCell(worldPos);
        return new Vector3Int(c.x, 0, c.z);
    }

    public Vector3 CellCenter(Vector3Int cell)
    {
        Vector3 p = grid.GetCellCenterWorld(new Vector3Int(cell.x, 0, cell.z));
        p.y = groundTopY + settings.yOffset;
        return p;
    }

    public bool InBounds(Vector3Int c)
    {
        return c.x >= MinCell.x && c.x <= MaxCell.x && c.z >= MinCell.z && c.z <= MaxCell.z;
    }

    public Vector3Int RandomCellInBounds()
    {
        return new Vector3Int(Random.Range(MinCell.x, MaxCell.x + 1), 0,
                              Random.Range(MinCell.z, MaxCell.z + 1));
    }

    public int NextPersonId() { return nextPersonId++; }

    // ---------- walkability ----------
    // A cell is walkable for a person if it is inside the ground, has no tree,
    // isn't occupied by another person, and isn't another person's reserved destination.
    public bool IsWalkable(Vector3Int cell, int personId)
    {
        if (!InBounds(cell)) return false;
        if (TreeGrid.IsOccupied(cell)) return false;
        if (PeopleGrid.IsOccupiedByOther(cell, personId)) return false;
        if (IsReservedByOther(cell, personId)) return false;
        return true;
    }

    // ---------- destination reservation ----------
    public void Reserve(Vector3Int cell, int personId) { reserved[cell] = personId; }

    public void ReleaseReservation(Vector3Int cell, int personId)
    {
        if (reserved.TryGetValue(cell, out int owner) && owner == personId) reserved.Remove(cell);
    }

    public bool IsReservedByOther(Vector3Int cell, int personId)
    {
        return reserved.TryGetValue(cell, out int owner) && owner != personId;
    }

    public void ReleaseAllReservations(int personId)
    {
        var toRemove = new List<Vector3Int>();
        foreach (var kv in reserved) if (kv.Value == personId) toRemove.Add(kv.Key);
        foreach (var c in toRemove) reserved.Remove(c);
    }

    // ---------- pathfinding ----------
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goalCenter, int goalRadius, int personId)
    {
        return GridPathfinder.FindPath(start, goalCenter, goalRadius,
                                       c => IsWalkable(c, personId),
                                       MinCell, MaxCell, settings.maxPathNodes);
    }
}

// Small helpers built ONLY on GridData's existing public methods, so GridData.cs
// does not need to change. For the people grid we store the person's id in the
// "placedObjectIndex" slot, so GetRepresnetationIndex(cell) returns the owner id
// (or -1 when the cell is empty).
public static class GridDataCellExtensions
{
    public static bool IsOccupied(this GridData g, Vector3Int cell)
    {
        return !g.CanPlaceObjectAt(cell, Vector2Int.one);
    }

    public static bool IsOccupiedByOther(this GridData g, Vector3Int cell, int ownerId)
    {
        int owner = g.GetRepresnetationIndex(cell);
        return owner != -1 && owner != ownerId;
    }

    public static bool TryOccupy(this GridData g, Vector3Int cell, int ownerId)
    {
        if (g.IsOccupied(cell)) return false;
        g.AddObjectAt(cell, Vector2Int.one, 0, ownerId);
        return true;
    }

    public static void Vacate(this GridData g, Vector3Int cell, int ownerId)
    {
        if (g.GetRepresnetationIndex(cell) == ownerId) g.RemoveObjectAt(cell);
    }
}