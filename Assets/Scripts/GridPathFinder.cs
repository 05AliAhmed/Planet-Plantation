using System;
using System.Collections.Generic;
using UnityEngine;

// A* on the XZ grid (cells are Vector3Int(x, 0, z), same as GridData).
// 8-directional, no corner cutting. The goal is "any walkable cell within
// goalRadius (Chebyshev) of goalCenter":
//   radius 0 -> exactly that cell
//   radius 1 -> any cell touching it (used to stand next to a tree)
public static class GridPathfinder
{
    private const int Straight = 10;
    private const int Diagonal = 14;

    private static readonly Vector3Int[] Dirs =
    {
        new Vector3Int( 1, 0,  0), new Vector3Int(-1, 0,  0),
        new Vector3Int( 0, 0,  1), new Vector3Int( 0, 0, -1),
        new Vector3Int( 1, 0,  1), new Vector3Int( 1, 0, -1),
        new Vector3Int(-1, 0,  1), new Vector3Int(-1, 0, -1),
    };

    // Returns the cells to walk through (start NOT included, goal cell included).
    // Empty list = already at goal. null = no path.
    public static List<Vector3Int> FindPath(Vector3Int start,
                                            Vector3Int goalCenter,
                                            int goalRadius,
                                            Func<Vector3Int, bool> isWalkable,
                                            Vector3Int min,
                                            Vector3Int max,
                                            int maxNodes = 3000)
    {
        if (IsGoal(start, goalCenter, goalRadius)) return new List<Vector3Int>();

        var open = new MinHeap();
        var gCost = new Dictionary<Vector3Int, int>();
        var parent = new Dictionary<Vector3Int, Vector3Int>();
        var closed = new HashSet<Vector3Int>();

        gCost[start] = 0;
        open.Push(Heuristic(start, goalCenter, goalRadius), start);

        int expanded = 0;
        while (open.Count > 0 && expanded < maxNodes)
        {
            Vector3Int cur = open.Pop();
            if (!closed.Add(cur)) continue;     // stale heap entry
            expanded++;

            if (IsGoal(cur, goalCenter, goalRadius))
                return Reconstruct(parent, start, cur);

            foreach (Vector3Int d in Dirs)
            {
                Vector3Int n = cur + d;
                if (n.x < min.x || n.x > max.x || n.z < min.z || n.z > max.z) continue;
                if (closed.Contains(n)) continue;
                if (!isWalkable(n)) continue;

                bool diagonal = d.x != 0 && d.z != 0;
                if (diagonal)
                {
                    // don't squeeze diagonally between two blocked cells
                    if (!isWalkable(cur + new Vector3Int(d.x, 0, 0))) continue;
                    if (!isWalkable(cur + new Vector3Int(0, 0, d.z))) continue;
                }

                int ng = gCost[cur] + (diagonal ? Diagonal : Straight);
                if (!gCost.TryGetValue(n, out int old) || ng < old)
                {
                    gCost[n] = ng;
                    parent[n] = cur;
                    open.Push(ng + Heuristic(n, goalCenter, goalRadius), n);
                }
            }
        }
        return null;
    }

    private static bool IsGoal(Vector3Int c, Vector3Int goal, int radius)
    {
        return Mathf.Abs(c.x - goal.x) <= radius && Mathf.Abs(c.z - goal.z) <= radius;
    }

    // Octile distance, reduced by the goal radius so it never overestimates.
    private static int Heuristic(Vector3Int c, Vector3Int goal, int radius)
    {
        int dx = Mathf.Abs(c.x - goal.x);
        int dz = Mathf.Abs(c.z - goal.z);
        int h = Straight * (dx + dz) + (Diagonal - 2 * Straight) * Mathf.Min(dx, dz);
        return Mathf.Max(0, h - radius * Diagonal);
    }

    private static List<Vector3Int> Reconstruct(Dictionary<Vector3Int, Vector3Int> parent,
                                                Vector3Int start, Vector3Int goal)
    {
        var path = new List<Vector3Int>();
        Vector3Int cur = goal;
        while (cur != start)
        {
            path.Add(cur);
            cur = parent[cur];
        }
        path.Reverse();
        return path;
    }

    // Tiny binary min-heap (Unity has no PriorityQueue).
    private class MinHeap
    {
        private struct Entry { public int f; public Vector3Int cell; }
        private readonly List<Entry> items = new List<Entry>();

        public int Count => items.Count;

        public void Push(int f, Vector3Int cell)
        {
            items.Add(new Entry { f = f, cell = cell });
            int i = items.Count - 1;
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (items[p].f <= items[i].f) break;
                (items[p], items[i]) = (items[i], items[p]);
                i = p;
            }
        }

        public Vector3Int Pop()
        {
            Vector3Int top = items[0].cell;
            int last = items.Count - 1;
            items[0] = items[last];
            items.RemoveAt(last);
            int i = 0;
            while (true)
            {
                int l = 2 * i + 1, r = l + 1, s = i;
                if (l < items.Count && items[l].f < items[s].f) s = l;
                if (r < items.Count && items[r].f < items[s].f) s = r;
                if (s == i) break;
                (items[s], items[i]) = (items[i], items[s]);
                i = s;
            }
            return top;
        }
    }
}