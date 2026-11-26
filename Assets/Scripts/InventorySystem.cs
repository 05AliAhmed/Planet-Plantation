using System;
using System.Collections.Generic;
using UnityEngine;

// Tracks how many of each tree the player owns. Counts live in PlayerData.Inventory
// (tree ID -> count), so they carry over between levels.
// ObjDatabaseSO stays the catalogue of what exists; this only stores what you OWN.
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Serializable]
    public class StockEntry
    {
        public int id;          // tree ID from ObjDatabaseSO
        public int amount = 1;
    }

    [SerializeField] private ObjDatabaseSO database;
    [Tooltip("Applied only the first time the game starts, not on every level load.")]
    [SerializeField] private List<StockEntry> startingStock = new List<StockEntry>();

    // (tree ID, new count). UI and shop can subscribe to refresh themselves.
    public event Action<int, int> OnCountChanged;

    private void Awake()
    {
        Instance = this;

        if (PlayerData.InventoryInitialised) return;
        PlayerData.InventoryInitialised = true;

        foreach (StockEntry s in startingStock)
        {
            if (!AddItem(s.id, s.amount))
                Debug.LogWarning($"InventorySystem: starting stock entry with ID {s.id} was ignored (unknown ID or amount <= 0).", this);
        }
    }

    // ---------- the three functions you asked for ----------

    // How many trees with this ID the player owns.
    public int TotalCount(int id)
    {
        return PlayerData.Inventory.TryGetValue(id, out int count) ? count : 0;
    }

    // Adds trees with this ID. Returns false for an unknown ID or amount <= 0.
    public bool AddItem(int id, int amount = 1)
    {
        if (amount <= 0 || !IsValidID(id)) return false;
        int newCount = TotalCount(id) + amount;
        PlayerData.Inventory[id] = newCount;
        OnCountChanged?.Invoke(id, newCount);
        return true;
    }

    // Removes trees with this ID. Returns false (and changes nothing) if you don't have enough.
    public bool RemoveItem(int id, int amount = 1)
    {
        if (amount <= 0) return false;
        int have = TotalCount(id);
        if (have < amount) return false;
        int newCount = have - amount;
        PlayerData.Inventory[id] = newCount;
        OnCountChanged?.Invoke(id, newCount);
        return true;
    }

    // ---------- helpers ----------

    public bool HasItem(int id) { return TotalCount(id) > 0; }

    public bool IsValidID(int id)
    {
        return database != null && database.objsData.Exists(o => o.ID == id);
    }

    // Tree name from the database, for messages like "Need more Oak trees".
    public string GetName(int id)
    {
        if (database == null) return "ID " + id;
        ObjectData data = database.objsData.Find(o => o.ID == id);
        return data != null ? data.Name : "ID " + id;
    }
}