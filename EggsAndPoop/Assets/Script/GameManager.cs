using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent m_EggCrackedOpen;
    public UnityEvent m_SetupEgg;

    public UnityEvent m_EggContextOpened;
    public UnityEvent m_EggContextClosed;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TestEggDropping()
    {
        var droppedAnimal = AnimalRedeeming.Instance.OpenEgg();
        var rarityString = System.Enum.GetName(typeof(AnimalRarity), droppedAnimal.rarity);
        print($"You opened a {AnimalRedeeming.Instance.activeEgg.eggName} and found a {rarityString} {droppedAnimal.animalName}");
    }
}
