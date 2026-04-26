using UnityEngine;

public class PoopManager : MonoBehaviour
{
    public static PoopManager Instance;

    public InventoryConfig inventoryConfig;

    private int _poopAmount;
    private int _poopCapacity;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadPoop()
    {
        var saveData = DataController.instance.GetData();
        _poopAmount = saveData.poopAmount;
        _poopCapacity = saveData.poopCapacity > 0
            ? saveData.poopCapacity
            : inventoryConfig.basePoopCapacity + FarmManager.Instance.GetBonusPoopCapacity();
    }

    public void AddPoop(int amount)
    {
        _poopAmount = Mathf.Min(_poopAmount + amount, GetPoopCapacity());
    }

    public void SellPoop(int amount)
    {
        amount = Mathf.Min(amount, _poopAmount);
        if (amount <= 0) return;
        _poopAmount -= amount;
        CoinManager.Instance.AddCoins(Mathf.FloorToInt(amount * inventoryConfig.poopToCoinsRate));
        DataController.instance.CompleteSave();
    }

    public void CompostPoop(int amount)
    {
        amount = Mathf.Min(amount, _poopAmount);
        if (amount <= 0) return;
        int fertilizer = Mathf.FloorToInt(amount / inventoryConfig.poopToFertilizerRatio);
        if (fertilizer <= 0) return;
        _poopAmount -= amount;
        FertilizerManager.Instance.AddFertilizer(fertilizer);
        DataController.instance.CompleteSave();
    }

    public void MulchBreedingPen(int amount)
    {
        amount = Mathf.Min(amount, _poopAmount);
        if (amount <= 0) return;
        _poopAmount -= amount;
        EnclosureManager.Instance.ApplyMulchBonus(amount * inventoryConfig.poopMulchBonusPerUnit);
        DataController.instance.CompleteSave();
    }

    public int GetPoopAmount() => _poopAmount;
    public int GetPoopCapacity() => Mathf.Max(_poopCapacity, inventoryConfig.basePoopCapacity + FarmManager.Instance.GetBonusPoopCapacity());

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.poopAmount = _poopAmount;
        saveData.poopCapacity = GetPoopCapacity();
    }
}
