using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private int _coins;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadCoins()
    {
        _coins = DataController.instance.GetData().money;
    }

    public void AddCoins(int amount)
    {
        _coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (_coins < amount) return false;
        _coins -= amount;
        return true;
    }

    public int GetCoins() => _coins;

    public static string FormatCoins(int copper)
    {
        int gold   = copper / 10000;
        int silver = (copper % 10000) / 100;
        int rem    = copper % 100;

        if (gold > 0)   return $"{gold}g {silver}s";
        if (silver > 0) return $"{silver}s {rem}c";
        return $"{rem}c";
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.money = _coins;
    }
}
