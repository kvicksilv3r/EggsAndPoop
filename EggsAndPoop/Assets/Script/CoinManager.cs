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

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.money = _coins;
    }
}
