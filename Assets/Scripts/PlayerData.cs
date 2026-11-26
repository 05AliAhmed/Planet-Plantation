using System.Collections.Generic;

// Data that must survive scene changes (levels 2-5, menus, etc).
// A static class lives for the whole play session, so it needs no GameObject and no
// DontDestroyOnLoad. Per-scene managers (MoneyManager, InventorySystem) read and write
// it, and keep their own scene-specific UI references.
public static class PlayerData
{
    public static float Money;
    public static bool MoneyInitialised;      // starting money is applied only once per session

    // tree ID -> how many the player owns
    public static readonly Dictionary<int, int> Inventory = new Dictionary<int, int>();
    public static bool InventoryInitialised;  // starting stock is applied only once per session

    // Call from a "New Game" button to wipe progress.
    public static void ResetAll()
    {
        Money = 0f;
        MoneyInitialised = false;
        Inventory.Clear();
        InventoryInitialised = false;
    }
}
