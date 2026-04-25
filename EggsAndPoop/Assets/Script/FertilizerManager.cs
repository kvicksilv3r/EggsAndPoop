using System;
using UnityEngine;

public class FertilizerManager : MonoBehaviour
{
    public static FertilizerManager Instance;

    public InventoryConfig inventoryConfig;

    private int fertilizerAmount;
    private int fertilizerCapacity;
    private int foodAmount;
    private int foodCapacity;

    public float CachedHoursAway { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadFertilizer()
    {
        var saveData = DataController.instance.GetData();

        fertilizerAmount = saveData.fertilizerAmount;
        fertilizerCapacity = saveData.fertilizerCapacity > 0
            ? saveData.fertilizerCapacity
            : inventoryConfig.baseFertilizerCapacity;

        foodAmount = saveData.foodAmount;
        foodCapacity = saveData.foodCapacity > 0
            ? saveData.foodCapacity
            : inventoryConfig.baseFoodCapacity;

        // Snapshot time away before EnclosureManager.Initialize() triggers a CompleteSave
        CachedHoursAway = (float)(DateTime.Now - saveData.lastTimeActive.ToDateTime()).TotalHours;
    }

    public void AddFertilizer(int amount)
    {
        fertilizerAmount = Mathf.Min(fertilizerAmount + amount, fertilizerCapacity);
    }

    public void ConvertToFood(int fertilizerToSpend)
    {
        fertilizerToSpend = Mathf.Min(fertilizerToSpend, fertilizerAmount);
        int foodGained = Mathf.FloorToInt(fertilizerToSpend / inventoryConfig.fertilizerToFoodRatio);

        if (foodGained <= 0) return;

        fertilizerAmount -= fertilizerToSpend;
        foodAmount = Mathf.Min(foodAmount + foodGained, foodCapacity);
    }

    public bool ConsumeFood(int amount)
    {
        if (foodAmount < amount) return false;
        foodAmount -= amount;
        return true;
    }

    public int GetFertilizerAmount() => fertilizerAmount;
    public int GetFertilizerCapacity() => fertilizerCapacity;
    public bool FertilizerIsFull() => fertilizerAmount >= fertilizerCapacity;
    public int GetFoodAmount() => foodAmount;
    public int GetFoodCapacity() => foodCapacity;
    public bool HasFood() => foodAmount > 0;

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.fertilizerAmount = fertilizerAmount;
        saveData.fertilizerCapacity = fertilizerCapacity;
        saveData.foodAmount = foodAmount;
        saveData.foodCapacity = foodCapacity;
    }
}
