using UnityEngine;

// Create via: Assets > Create > People > People Settings
// All people tuning lives here so you never have to touch code to rebalance.
[CreateAssetMenu(menuName = "People/People Settings", fileName = "PeopleSettings")]
public class PeopleSettingSO : ScriptableObject
{
    [Header("Spawning")]
    public int spawnCount = 5;
    public int spawnAttempts = 50;          // tries to find a free cell per person

    [Header("Roaming")]
    [Min(1)] public int roamRadius = 4;     // ranged area = (2*radius+1) x (2*radius+1) cells
    public float waitMin = 3f;              // wait at each roam cell
    public float waitMax = 4f;
    public int maxRoamsBeforeRest = 0;      // 0 = never go home to rest unless after a tree

    [Header("Resting")]
    public float restTime = 60f;

    [Header("Movement")]
    public float moveSpeed = 2f;            // world units per second
    public float yOffset = 0f;              // lift people above the ground top
    public int homeSearchRadius = 3;        // if home is taken, settle within this many cells
    public int maxRepaths = 5;              // re-plan attempts when a cell gets blocked
    public float blockedRetryDelay = 0.3f;
    public int maxPathNodes = 3000;         // safety cap for the pathfinder

    [Header("Trees")]
    public float treeCooldown = 30f;        // a tree can't be targeted again for this long
}