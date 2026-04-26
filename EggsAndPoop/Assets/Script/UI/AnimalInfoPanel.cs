using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInfoPanel : MonoBehaviour
{
    public static AnimalInfoPanel Instance;

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI sexText;
    public TextMeshProUGUI quirkText;
    public Button changeNameButton;
    public Button sendToStorageButton;
    public Button closeButton;

    private string currentGuid;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        sendToStorageButton.onClick.AddListener(SendToStorage);
        closeButton.onClick.AddListener(Hide);
        changeNameButton.onClick.AddListener(OpenNameEdit);
        nameInputField.onEndEdit.AddListener(CommitNameChange);
    }

    public void Show(string guid, string animalName)
    {
        currentGuid = guid;

        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);

        nameText.text = animalName;
        nameText.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(false);

        ageText.text = FormatAge(entry.timeOfBirth);
        if (sexText != null)
            sexText.text = entry.sex == AnimalSex.Female ? "♀ Girl" : "♂ Boy";
        quirkText.text = FormatQuirk(entry);

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        currentGuid = null;
    }

    private void OpenNameEdit()
    {
        nameInputField.text = nameText.text;
        nameText.gameObject.SetActive(false);
        nameInputField.gameObject.SetActive(true);
        nameInputField.ActivateInputField();
    }

    private void CommitNameChange(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName) && currentGuid != null)
        {
            var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == currentGuid);
            if (entry != null)
            {
                entry.customAnimalName = newName;
                DataController.instance.CompleteSave();
            }
        }

        nameText.text = string.IsNullOrWhiteSpace(newName) ? nameText.text : newName;
        nameInputField.gameObject.SetActive(false);
        nameText.gameObject.SetActive(true);
    }

    private void SendToStorage()
    {
        if (currentGuid == null) return;
        PlayerInventoryManager.Instance.MoveAnimalToStorage(currentGuid);
        Hide();
    }

    private string FormatAge(long timeOfBirthTicks)
    {
        var age = DateTime.Now - new DateTime(timeOfBirthTicks);

        if (age.TotalDays < 1) return "Born today";
        if (age.TotalDays < 2) return "1 day old";
        return $"{(int)age.TotalDays} days old";
    }

    private string FormatQuirk(PlayerAnimalEntry entry)
    {
        if (entry.quirkId < 0) return "";

        var age = DateTime.Now - new DateTime(entry.timeOfBirth);
        var daysRequired = PlayerInventoryManager.Instance.inventoryConfig.daysUntilQuirkUnlocks;

        if (age.TotalDays < daysRequired)
        {
            var daysLeft = daysRequired - (int)age.TotalDays;

            if (age.TotalDays < 1) return "Just arrived and a little overwhelmed...";
            if (daysLeft == 1) return "Starting to feel at home. Almost ready to open up...";
            return "Still a little shy. Give them some time...";
        }

        return PlayerInventoryManager.Instance.quirkData.quirks[entry.quirkId];
    }
}
