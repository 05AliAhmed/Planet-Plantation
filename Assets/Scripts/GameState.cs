using UnityEngine;

public static class GameState
{
    public static bool PanelOpen;   // set by UIManager
    public static bool Placing;     // set by PlacementSys
    public static bool IsPaused => PanelOpen || Placing;
}