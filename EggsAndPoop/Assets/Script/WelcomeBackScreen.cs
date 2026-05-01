using System.Collections;
using TMPro;
using UnityEngine;

public class WelcomeBackScreen : MonoBehaviour
{
    public TextMeshProUGUI welcomeBackTMP;
    public TextMeshProUGUI earnedEggsTMP;
    public GameObject welcomeBackPanel;

    public string prefKey = "PlayedGame";

    public string baseAfkText = "You have been away for";

    public void UpdateText()
    {
        var timeAway = System.TimeSpan.FromHours(FertilizerManager.Instance.CachedHoursAway);

        string days = timeAway.Days > 0 ? timeAway.Days + "d " : "";
        string hours = timeAway.Hours > 0 ? timeAway.Hours + "h " : "";
        string minutes = timeAway.Minutes > 0 ? timeAway.Minutes + "m " : "";
        string seconds = timeAway.Seconds > 0 ? timeAway.Seconds + "s" : "";

        welcomeBackTMP.text = $"{baseAfkText} \n {days}{hours}{minutes}{seconds}";

        var earnedEggs = EnclosureManager.Instance.LastSessionEggsEarned
                       + EggSlotManager.Instance.LastSessionEggsEarned;

        if (earnedEggs == 0)
        {
            earnedEggsTMP.text = PlayerInventoryManager.Instance.HasMaxEggs()
                ? "Your egg basket is full. Time to hatch some!"
                : $"No new eggs yet. Next one in {EggSlotManager.Instance.TimeUntilNextEgg()}";
        }
        else
        {
            earnedEggsTMP.text = earnedEggs > 1
                ? $"Your animals have been busy! {earnedEggs} new eggs waiting for you."
                : "One of your animals left you a little surprise. A new egg!";
        }
    }

    private void Awake()
    {
        GameManager.Instance.m_AfkDataProcessed.AddListener(Display);
    }

    public void Display()
    {
        if (!PlayerPrefs.HasKey(prefKey))
        {
            PlayerPrefs.SetInt(prefKey, 1);
            PlayerPrefs.Save();
            return;
        }

        welcomeBackPanel.SetActive(true);
        UpdateText();
        StartCoroutine(LiveCountdown());
    }

    private IEnumerator LiveCountdown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            UpdateText();
        }
    }

    public void Hide()
    {
        Destroy(welcomeBackPanel);
        Destroy(this);
    }
}
