using UnityEngine;

public class BarnPanel : MonoBehaviour
{
    public static BarnPanel Instance;

    public Transform listContainer;
    public GameObject animalEntryPrefab;
    public GameObject emptyLabel;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (PlayerInventoryManager.Instance == null) return;
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        var animals = PlayerInventoryManager.Instance.GetStorageAnimals();

        if (emptyLabel != null)
            emptyLabel.SetActive(animals.Count == 0);

        foreach (var entry in animals)
        {
            var obj = Instantiate(animalEntryPrefab, listContainer);
            obj.GetComponent<BarnAnimalEntry>().Setup(entry);
        }
    }
}
