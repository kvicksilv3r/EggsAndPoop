using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "EAP/New AnimalData")]
public class AnimalData : ScriptableObject
{
    public GameObject prefab;
    public string animalName;
    public string description;
    public AnimalRarity rarity;
    public AnimalFamily family;
    public AnimalPrimalInstinct behaviour;
    public AnimalAge age;
    public AnimalData[] possibleEvolutions;
    public float openingExtraScale = 1f;
    public int sellPrice = 0;
    public AnimalEnum animalIdentifier;
    public float maxWalkDistance = 10;
    public float movementSpeed = 2f;
    public float acceleration = 5;
    [Tooltip("Poops produced per hour")]
    public float poopRate = 1f;
    [Tooltip("Eggs contributed per hour when in the Breeding Pen")]
    public float eggContribution = 1f;
}
