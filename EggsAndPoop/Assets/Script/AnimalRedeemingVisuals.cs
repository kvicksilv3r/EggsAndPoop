using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimalRedeemingVisuals : MonoBehaviour
{
    public static AnimalRedeemingVisuals Instance;

    public Transform animalParent;
    private GameObject spawnedAnimal;
    private Vector3 animalBaseSize = Vector3.zero;

    public float animalAnimTime = 1f;
    public Vector3 animalRotationAmount = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameManager.Instance.m_SetupEgg.AddListener(RemoveAnimal);
        GameManager.Instance.m_EggContextClosed.AddListener(RemoveAnimal);
    }

    public void DisaplayAnimal(AnimalData animal)
    {
        if (spawnedAnimal != null)
        {
            RemoveAnimal();
        }

        spawnedAnimal = Instantiate(animal.prefab, animalParent);
        animalBaseSize = spawnedAnimal.transform.localScale;
        spawnedAnimal.transform.localScale = Vector3.zero;
        spawnedAnimal.transform.DOScale(animalBaseSize * animal.openingExtraScale, animalAnimTime);
        spawnedAnimal.transform.DORotate(animalParent.transform.rotation.eulerAngles + animalRotationAmount, animalAnimTime);
    }

    private IEnumerator AnimateAnimal()
    {
        yield return null;
    }

    public void RemoveAnimal()
    {
        if (spawnedAnimal == null)
        {
            return;
        }

        DestroyImmediate(spawnedAnimal);
    }
}
