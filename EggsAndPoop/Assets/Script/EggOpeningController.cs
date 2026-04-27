using System;
using UnityEngine;

public class EggOpeningController : MonoBehaviour
{
    public static EggOpeningController Instance { get; private set; }

    private AnimalData generatedAnimal;
    private EggData eggData;

    public GameObject animalInfoBox;

    private bool hasMaxEggs = false;

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.m_EggCrackedOpen.AddListener(EggCrackedOpen);
    }

    public EggData GetEgg()
    {
        return eggData;
    }

    public void InitiateEggOpening(EggData data)
    {
        if (!PlayerInventoryManager.Instance.HasOpenAnimalSlots())
        {
            FloatingMessageController.Instance.Show("Your farm and barn are full. Make some room first!");
            return;
        }

        eggData = data;
        hasMaxEggs = PlayerInventoryManager.Instance.HasMaxEggs();
        EggHelper.Instance.SetupEgg(eggData);
        GameManager.Instance.m_SetupEgg.Invoke();
    }

    public void EggCrackedOpen()
    {
        FetchAnimal();
        DisplayOpenedAnimal();
        RegisterNewAnimal();
        RemoveEggFromInventory();
        DisplayAnimalInformation();

        GameManager.Instance.m_EggCrackedOpenPost.Invoke();
    }

    private void DisplayAnimalInformation()
    {
        animalInfoBox.GetComponent<AnimalHatchedInformation>().DisplayInfo(generatedAnimal);
    }

    public void FetchAnimal()
    {
        generatedAnimal = AnimalRedeeming.Instance.OpenEgg(eggData);
    }

    public void DisplayOpenedAnimal()
    {
        AnimalRedeemingVisuals.Instance.DisaplayAnimal(generatedAnimal);
    }

    public void RegisterNewAnimal()
    {
        bool farmWasFull = !PlayerInventoryManager.Instance.HasOpenActiveSlots();
        PlayerInventoryManager.Instance.AddAnimal(generatedAnimal);

        if (farmWasFull)
            FloatingMessageController.Instance.Show("Your farm is full — they're settling into the barn for now.");
    }

    public void RemoveEggFromInventory()
    {
        PlayerInventoryManager.Instance.RemoveEgg(eggData.eggType);
    }
}
