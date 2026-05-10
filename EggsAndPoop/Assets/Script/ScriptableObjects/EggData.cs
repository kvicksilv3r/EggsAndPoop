using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "EAP/New EggData")]
public class EggData : ScriptableObject
{
    public string eggName;
    public EggType eggType;
    public AnimalFamily[] containedAnimalTypes;
    public AnimalRarity eggMinRarity;
    public AnimalRarity eggMaxRarity;
    public GameObject eggPrefab;
    public GameObject nestEggPrefab;
    public Texture2D eggIcon;
    [Tooltip("Icon shown in the nest preference picker — falls back to eggIcon if left empty")]
    public Texture2D nestPickerIcon;

    [Header("Progression")]
    public int tier = 1;
    public int unlockCost = 0;
    [Tooltip("Love cost multiplier applied to any nest slot assigned this egg type")]
    public float nestLoveCostMultiplier = 1f;
    public EggUnlockCondition[] unlockConditions;
}
