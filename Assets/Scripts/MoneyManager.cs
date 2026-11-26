using System;
using TMPro;
using UnityEngine;

// Money now lives in PlayerData.Money, so it carries over between levels.
// This component only handles the scene's UI text and the rules for adding/spending.
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [SerializeField] TMP_Text moneyTxt;
    [Tooltip("Applied only the first time the game starts, not on every level load.")]
    [SerializeField] private float startingMoney = 100f;

    public event Action<float> OnMoneyChanged;

    public float Money => PlayerData.Money;

    private void Awake()
    {
        Instance = this;

        if (!PlayerData.MoneyInitialised)
        {
            PlayerData.MoneyInitialised = true;
            PlayerData.Money = startingMoney;
        }
    }

    private void Start()
    {
        UpdateText();   // show the carried-over amount right away
    }

    // Income (trees call this). Skipped while paused (panel open / placing).
    // Pass ignorePause: true for gains that should always count, e.g. rewards.
    public void AddMoney(float amount, bool ignorePause = false)
    {
        if (!ignorePause && GameState.IsPaused) return;
        PlayerData.Money += amount;
        Changed();
    }

    public bool CanAfford(float cost)
    {
        return PlayerData.Money >= cost;
    }

    // For the shop later. Returns false (and changes nothing) if you can't afford it.
    // Not blocked by pause, because the shop panel itself pauses the game.
    public bool SpendMoney(float cost)
    {
        if (cost < 0f || !CanAfford(cost)) return false;
        PlayerData.Money -= cost;
        Changed();
        return true;
    }

    private void Changed()
    {
        UpdateText();
        OnMoneyChanged?.Invoke(PlayerData.Money);
    }

    private void UpdateText()
    {
        if (moneyTxt != null) moneyTxt.text = PlayerData.Money.ToString("F0");
    }
}