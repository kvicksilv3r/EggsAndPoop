using System;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public InventoryConfig inventoryConfig;

    private int _foodAmount;
    private int _foodCapacity;

    public float CachedHoursAway { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadFood()
    {
        var saveData = DataController.instance.GetData();

        _foodAmount = saveData.foodAmount;
        _foodCapacity = saveData.foodCapacity > 0
            ? saveData.foodCapacity
            : inventoryConfig.baseFoodCapacity;

        // Snapshot time away before EnclosureManager.Initialize() triggers a CompleteSave
        CachedHoursAway = (float)(DateTime.Now - saveData.lastTimeActive.ToDateTime()).TotalHours;
    }

    public void AddFood(int amount)
    {
        _foodAmount = Mathf.Min(_foodAmount + amount, _foodCapacity);
    }

    public bool ConsumeFood(int amount)
    {
        if (_foodAmount <= 0) return false;
        _foodAmount = Mathf.Max(_foodAmount - amount, 0);
        return true;
    }

    public int GetFoodAmount() => _foodAmount;
    public int GetFoodCapacity() => _foodCapacity;
    public bool HasFood() => _foodAmount > 0;

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.foodAmount = _foodAmount;
        saveData.foodCapacity = _foodCapacity;
    }
}
