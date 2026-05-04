using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarHUD : MonoBehaviour
{
    public static TopBarHUD Instance;

    [SerializeField] TextMeshProUGUI coinText;
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
        if (coinText != null)
            coinText.text = CoinManager.FormatCoins(CoinManager.Instance.GetCoins());
    }
}
