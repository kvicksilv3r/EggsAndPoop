using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimalData", menuName = "EAP/New AnimalData")]
public class AnimalData : ScriptableObject
{
    public GameObject prefab;
    public Texture2D icon;
    [FormerlySerializedAs("animalName")]
    public string speciesName;
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
    [Tooltip("Love generated per hour when in the Pasture")]
    [UnityEngine.Serialization.FormerlySerializedAs("eggContribution")]
    public float loveGeneration = 1f;
    [Tooltip("Force a specific sex, or leave Random for 50/50")]
    public AnimalSexOverride sexOverride = AnimalSexOverride.Random;
}
