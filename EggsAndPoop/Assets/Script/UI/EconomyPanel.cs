using TMPro;
using UnityEngine;

public class EconomyPanel : MonoBehaviour
{
    public static EconomyPanel Instance;

    public GameObject panel;
    public TextMeshProUGUI poopText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI fertilizerText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI mulchBonusText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(true);
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
        poopText.text = $"Poop: {PoopManager.Instance.GetPoopAmount()} / {PoopManager.Instance.GetPoopCapacity()}";
        coinsText.text = $"Coins: {CoinManager.Instance.GetCoins()}";
        fertilizerText.text = $"Fertilizer: {FertilizerManager.Instance.GetFertilizerAmount()} / {FertilizerManager.Instance.GetFertilizerCapacity()}";
        foodText.text = $"Food: {FertilizerManager.Instance.GetFoodAmount()} / {FertilizerManager.Instance.GetFoodCapacity()}";

        float bonus = EnclosureManager.Instance.GetBreedingRarityBonus();
        if (mulchBonusText != null)
            mulchBonusText.text = bonus > 0 ? $"Breeding bonus: +{bonus}" : "";
    }

    public void OnSellAll()
    {
        PoopManager.Instance.SellPoop(PoopManager.Instance.GetPoopAmount());
        RefreshUI();
    }

    public void OnCompostAll()
    {
        PoopManager.Instance.CompostPoop(PoopManager.Instance.GetPoopAmount());
        RefreshUI();
    }

    public void OnMulchAll()
    {
        PoopManager.Instance.MulchBreedingPen(PoopManager.Instance.GetPoopAmount());
        RefreshUI();
    }
}
