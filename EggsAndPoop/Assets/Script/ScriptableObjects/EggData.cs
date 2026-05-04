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
}
