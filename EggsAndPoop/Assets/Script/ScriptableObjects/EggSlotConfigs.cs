using UnityEngine;

[System.Serializable]
public class EggSlotConfig
{
    public float baseLoveCost = 200f;
    public float loveReductionPerUpgradeLevel = 20f;
    public int maxLoveReductionLevels = 5;
    public float luckBonusPerLevel = 10f;
    public int maxLuckLevels = 5;
}

[System.Serializable]
public class CartonTierConfig
{
    public float loveCost = 2000f;
}
