using System;
using UnityEngine;

public class EggTimer : MonoBehaviour
{
    public DateTime nextEggTime;
    public static EggTimer Instance;
    public InventoryConfig config;

    private TimeSpan timeAway;

    private int newEggs = 0;

    //Disgusting ticks (long) to DateTime conversions in here. I have sinned

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.m_SaveDataLoaded.AddListener(SetupTimer);
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(CheckSetNextEgg);
    }

    public void SetupTimer()
    {
        nextEggTime = new DateTime(DataController.instance.GetData().nextEggTime);
        CalculateAfkTime();
    }

    public void CalculateAfkTime()
    {
        print("Calculating afk time");
        var lastTimeActiveTicks = DataController.instance.GetData().lastTimeActive;
        var lastTimeActive = new DateTime(lastTimeActiveTicks);
        timeAway = DateTime.Now - lastTimeActive;
        newEggs = (int)timeAway.TotalHours / 2;

        if (newEggs == 0)
        {
            var convertedNextEggTime = new DateTime(DataController.instance.GetData().nextEggTime);
            if (convertedNextEggTime.CompareTo(DateTime.Now) < 0)
            {
                newEggs = 1;
                SetNextEggTime(convertedNextEggTime);
            }
        }

        var maxEggs = PlayerInventoryManager.Instance.GetMaxEggCapacity() - PlayerInventoryManager.Instance.GetCurrentEggCount();

        if (newEggs > maxEggs)
        {
            newEggs = maxEggs;
        }

        GameManager.Instance.m_AfkDataProcessed.Invoke();
    }

    public void SetNextEggTime()
    {
        nextEggTime = DateTime.Now.AddHours(config.hoursForNewEgg);
    }

    public void SetNextEggTime(DateTime referenceTime)
    {
        nextEggTime = referenceTime.AddHours(config.hoursForNewEgg);
    }

    public TimeSpan TimeSpanUntilNextEgg()
    {
        return nextEggTime.Subtract(DateTime.Now);
    }

    public string StringUntilNextTime()
    {
        var untilEgg = TimeSpanUntilNextEgg();
        string hours = untilEgg.Hours > 0 ? untilEgg.Hours.ToString() + "h " : "";
        string minutes = untilEgg.Minutes > 0 ? untilEgg.Minutes.ToString() + "m " : "";
        string seconds = untilEgg.Seconds > 0 ? untilEgg.Seconds.ToString() + "s" : "";

        return hours + minutes + seconds;
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.nextEggTime = nextEggTime.Ticks;
    }

    public TimeSpan GetTimeAway()
    {
        return timeAway;
    }

    public int GetNewEggs()
    {
        return newEggs;
    }

    private void CheckSetNextEgg()
    {
        if (nextEggTime.CompareTo(DateTime.Now) < 0)
        {
            SetNextEggTime();
        }
    }
}
