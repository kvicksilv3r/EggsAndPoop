using System;
using TMPro;
using UnityEngine;

public class BarnAnimalEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speciesText;
    public TextMeshProUGUI ageText;

    private string guid;

    public void Setup(PlayerAnimalEntry entry)
    {
        guid = entry.guid;
        nameText.text = entry.customAnimalName;

        var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
        speciesText.text = data != null ? data.speciesName : string.Empty;
        ageText.text = FormatAge(entry.timeOfBirth);
    }

    public void OnClick()
    {
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        if (entry != null)
            AnimalInfoPanel.Instance.Show(guid, entry.customAnimalName);
    }

    private string FormatAge(long ticks)
    {
        var age = DateTime.Now - new DateTime(ticks);
        if (age.TotalDays < 1) return "Born today";
        if (age.TotalDays < 2) return "1 day old";
        return $"{(int)age.TotalDays} days old";
    }
}
