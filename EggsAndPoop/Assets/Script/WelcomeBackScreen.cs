using TMPro;
using UnityEngine;

public class WelcomeBackScreen : MonoBehaviour
{
    public TextMeshProUGUI welcomeBackTMP;
    public TextMeshProUGUI earnedEggsTMP;
    public GameObject welcomeBackPanel;

    public string baseAfkText = "You have been away for";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.m_AfkDataProcessed.AddListener(Display);
    }

    public void UpdateText()
    {
        var timeAway = EggTimer.Instance.GetTimeAway();

        string days = timeAway.Days > 0 ? timeAway.Days.ToString() + "d " : "";
        string hours = timeAway.Hours > 0 ? timeAway.Hours.ToString() + "h " : "";
        string minutes = timeAway.Minutes > 0 ? timeAway.Minutes.ToString() + "m " : "";
        string seconds = timeAway.Seconds > 0 ? timeAway.Seconds.ToString() + "s" : "";

        welcomeBackTMP.text = $"{baseAfkText} \n {days}{hours}{minutes}{seconds}";

        var earnedEggs = EggTimer.Instance.GetNewEggs();

        if (earnedEggs == 0)
        {
            if (PlayerInventoryManager.Instance.HasMaxEggs())
            {
                earnedEggsTMP.text = "You've earned no new eggs \n (Egg storage full)";
            }

            else
            {
                earnedEggsTMP.text = $"You've earned no new eggs yet \n Next egg in {EggTimer.Instance.StringUntilNextTime()}";
            }
        }

        else
        {
            earnedEggsTMP.text = earnedEggs > 1 ? $"You've earned {earnedEggs} new eggs" : $"You've earned {earnedEggs} new egg";
        }
    }

    public void Display()
    {
        welcomeBackPanel.SetActive(true);
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    public void Hide()
    {
        Destroy(welcomeBackPanel);
        Destroy(this);
    }
}
