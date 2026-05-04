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
        var timeAway = System.TimeSpan.FromHours(FoodManager.Instance.CachedHoursAway);

        string days = timeAway.Days > 0 ? timeAway.Days + "d " : "";
        string hours = timeAway.Hours > 0 ? timeAway.Hours + "h " : "";
        string minutes = timeAway.Minutes > 0 ? timeAway.Minutes + "m " : "";
        string seconds = timeAway.Seconds > 0 ? timeAway.Seconds + "s" : "";

        welcomeBackTMP.text = $"{baseAfkText} \n {days}{hours}{minutes}{seconds}";

        var eggsAtNests = EggNestManager.Instance.GetUnclaimedEggCount();

        if (eggsAtNests == 0)
        {
            earnedEggsTMP.text = $"No new eggs yet. Next one in {EggSlotManager.Instance.TimeUntilNextEgg()}";
        }
        else if (eggsAtNests == 1)
        {
            earnedEggsTMP.text = "One of your animals left a little surprise at the nest!";
        }
        else
        {
            earnedEggsTMP.text = $"{eggsAtNests} eggs are waiting for you at the nests!";
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
