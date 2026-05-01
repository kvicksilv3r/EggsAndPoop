using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarHUD : MonoBehaviour
{
    public static TopBarHUD Instance;

    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI poopText;
    [SerializeField] Image poopFillBar;
    [SerializeField] TextMeshProUGUI eggTimerText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameManager.Instance.m_SaveDataLoaded.AddListener(RefreshUI);
        StartCoroutine(RefreshCoroutine());
    }

    void Update()
    {
        if (EggSlotManager.Instance != null)
            eggTimerText.text = EggSlotManager.Instance.TimeUntilNextEgg();
    }

    IEnumerator RefreshCoroutine()
    {
        while (true)
        {
            RefreshUI();
            yield return new WaitForSeconds(1f);
        }
    }

    public void RefreshUI()
    {
        coinText.text = CoinManager.FormatCoins(CoinManager.Instance.GetCoins());

        int poop    = PoopManager.Instance.GetPoopAmount();
        int poopCap = PoopManager.Instance.GetPoopCapacity();
        poopText.text          = $"{poop}/{poopCap}";
        poopFillBar.fillAmount = poopCap > 0 ? (float)poop / poopCap : 0f;
    }
}
