using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "EAP/New EggData")]
public class EggData : ScriptableObject
{
    public string eggName;
    public AnimalFamily[] eggTypes;
    public AnimalRarity eggMinRarity;
    public AnimalRarity eggMaxRarity;
    public GameObject eggPrefab;
    public Texture2D eggIcon;
}
