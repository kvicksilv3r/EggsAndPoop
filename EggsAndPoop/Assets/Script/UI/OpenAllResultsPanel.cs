using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OpenAllResultsPanel : MonoBehaviour
{
    public static OpenAllResultsPanel Instance;

    public Transform listContainer;
    public GameObject animalEntryPrefab;

    public float entryPopDuration = 0.25f;
    public float entryStagger = 0.06f;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(List<PlayerAnimalEntry> newAnimals)
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        gameObject.SetActive(true);

        for (int i = 0; i < newAnimals.Count; i++)
        {
            var obj = Instantiate(animalEntryPrefab, listContainer);
            obj.GetComponent<BarnAnimalEntry>().Setup(newAnimals[i]);
            obj.transform.localScale = Vector3.zero;
            obj.transform
                .DOScale(Vector3.one, entryPopDuration)
                .SetDelay(i * entryStagger)
                .SetEase(Ease.OutBack);
        }
    }

    public void Close()
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);
        gameObject.SetActive(false);
    }
}
