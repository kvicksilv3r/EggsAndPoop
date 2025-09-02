using UnityEngine;

public class EggOpeningController : MonoBehaviour
{
    public static EggOpeningController Instance { get; private set; }

    private AnimalData generatedAnimal;
    private EggData eggData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.m_EggCrackedOpen.AddListener(EggCrackedOpen);
    }

    public EggData GetEgg()
    {
        return eggData;
    }

    public void InitiateEggOpening(EggData data)
    {
        eggData = data;
        GameManager.Instance.m_SetupEgg.Invoke();
    }

    public void EggCrackedOpen()
    {
        FetchAnimal();
        DisplayOpenedAnimal();
        RegisterNewAnimal();
        RemoveEggFromInventory();
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
        //register animal to player inventory

    }

    public void RemoveEggFromInventory()
    {
        //remove egg from egg inventory
    }
}
