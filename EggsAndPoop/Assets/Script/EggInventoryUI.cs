using System;
using Unity.VisualScripting;
using UnityEngine;

public class EggInventoryUI : MonoBehaviour
{
    public Transform eggButtonHolster;
    public GameObject eggButton;

    //todo make this actually update each time


    private void Start()
    {
        CleanUI();
        SetupEggButtons();
    }


    private void CleanUI()
    {
        foreach (Transform button in eggButtonHolster.transform)
        {
            Destroy(button);
        }
    }

    public void SetupEggButtons()
    {
        var eggs = DataController.instance.playerEggs;

        foreach (var eggEntry in eggs)
        {
            var g = Instantiate(eggButton, eggButtonHolster);
            g.GetComponent<EggUiButton>().SetupButton(eggEntry);
        }
    }
}
