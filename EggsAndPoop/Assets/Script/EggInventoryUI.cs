using UnityEngine;

public class EggInventoryUI : MonoBehaviour
{
    public Transform eggButtonHolster;
    public GameObject eggButton;

    //todo make this actually update each time


    private void Start()
    {
        RefreshUI();
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(RefreshUI);
        GameManager.Instance.m_EggContextOpened.AddListener(RefreshUI);
    }

    private void CleanUI()
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
        SetupEggButtons();
    }
}
