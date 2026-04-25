using System;
using UnityEngine;

public class EggInventoryUI : MonoBehaviour
{
    public Transform eggButtonHolster;
    public GameObject eggButton;

    public GameObject noEggsPanel;
    public GameObject fullAnimalSlotsPanel;

    private void Awake()
    {
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(RefreshUI);
        GameManager.Instance.m_EggContextOpened.AddListener(RefreshUI);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void CleanUI()
    {
        DestroyEggButtons();
        HideFullAnimalInventory();
        HideOutOfEggs();
    }

    private void DestroyEggButtons()
    {
        foreach (Transform button in eggButtonHolster.transform)
        {
            Destroy(button.gameObject);
        }
    }

    public void SetupEggButtons()
    {
        var eggs = PlayerInventoryManager.Instance.playerEggs;

        foreach (var eggEntry in eggs)
        {
            var g = Instantiate(eggButton, eggButtonHolster);
            var eggData = EggRoster.Instance.GetByType(eggEntry.eggType);
            g.GetComponent<EggUiButton>().SetupButton(eggData, eggEntry.eggAmount);
        }
    }

    public void RefreshUI()
    {
        CleanUI();

        if (!PlayerInventoryManager.Instance.HasEggs())
        {
            DisplayOutOfEggs();
        }

        else if (!PlayerInventoryManager.Instance.HasOpenAnimalSlots())
        {
            DisplayFullAnimalInventory();
        }

        else
        {
            SetupEggButtons();
        }
    }

    private void DisplayFullAnimalInventory()
    {
        fullAnimalSlotsPanel.gameObject.SetActive(true);
    }

    private void HideFullAnimalInventory()
    {
        fullAnimalSlotsPanel.gameObject.SetActive(false);
    }

    private void DisplayOutOfEggs()
    {
        noEggsPanel.SetActive(true);
    }

    private void HideOutOfEggs()
    {
        noEggsPanel.SetActive(false);
    }
}
