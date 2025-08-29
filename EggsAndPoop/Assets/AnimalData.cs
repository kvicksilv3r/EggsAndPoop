using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "EAP/New AnimalData")]
public class AnimalData : ScriptableObject
{
    public GameObject prefab;
    public string animalName;
    public string description;
    public AnimalRarity rarity;
    public AnimalFamily family;
    public AnimalBehaviour behaviour;
}
