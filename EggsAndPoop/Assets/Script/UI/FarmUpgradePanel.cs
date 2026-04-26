using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmUpgradePanel : MonoBehaviour
{
    public static FarmUpgradePanel Instance;

    public GameObject panel;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI bonusesText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI coinsText;
    public Button upgradeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(true);
        panel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgrade);
    }

    public void Show()
    {
        RefreshUI();
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void RefreshUI()
    {
        int level = FarmManager.Instance.GetLevel();
        int cost = FarmManager.Instance.GetUpgradeCost();
        int coins = CoinManager.Instance.GetCoins();

        levelText.text = $"Farm Level {level}";
        bonusesText.text = FarmManager.Instance.GetBonusSummary();
        costText.text = $"Next upgrade: {cost} coins";
        coinsText.text = $"You have: {coins} coins";

        if (upgradeButton != null)
            upgradeButton.interactable = coins >= cost;
    }

    private void OnUpgrade()
    {
        FarmManager.Instance.TryUpgrade();
        RefreshUI();
    }
}
