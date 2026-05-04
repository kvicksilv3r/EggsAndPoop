using TMPro;
using UnityEngine;

public class EconomyPanel : MonoBehaviour
{
    public static EconomyPanel Instance;

    public GameObject panel;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI coinsText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
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
        if (foodText != null)
            foodText.text = $"Food: {FoodManager.Instance.GetFoodAmount()} / {FoodManager.Instance.GetFoodCapacity()}";
        if (coinsText != null)
            coinsText.text = $"Coins: {CoinManager.Instance.GetCoins()}";
    }
}
