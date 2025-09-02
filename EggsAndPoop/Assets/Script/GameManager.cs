using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void TestEggDropping()
    {
        var droppedAnimal = AnimalRedeeming.Instance.OpenEgg();
        var rarityString = System.Enum.GetName(typeof(AnimalRarity), droppedAnimal.rarity);
        print($"You opened a {AnimalRedeeming.Instance.activeEgg.eggName} and found a {rarityString} {droppedAnimal.animalName}");
    }
}
