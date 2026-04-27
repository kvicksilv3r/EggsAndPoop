using UnityEngine;

public class BarnPanel : MonoBehaviour
{
    public Transform listContainer;
    public GameObject animalEntryPrefab;
    public GameObject emptyLabel;

    private void OnEnable()
    {
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
