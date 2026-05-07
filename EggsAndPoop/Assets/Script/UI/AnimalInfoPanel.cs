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
    public TextMeshProUGUI happinessText;
    public Button changeNameButton;
    public Button favouriteButton;
    public TextMeshProUGUI favouriteButtonText;
    public Button sellButton;
    public Button sendToStorageButton;
    public TextMeshProUGUI sendToStorageButtonText;
    public Button closeButton;

    private const int MaxNameLength = 20;

    private string currentGuid;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(true);   // force active so children initialise
        panel.SetActive(false);  // then immediately hide
        nameInputField.characterLimit = MaxNameLength;
        nameInputField.gameObject.SetActive(false);
        sendToStorageButton.onClick.AddListener(ToggleStorage);
        closeButton.onClick.AddListener(Hide);
        changeNameButton.onClick.AddListener(OpenNameEdit);
        nameInputField.onEndEdit.AddListener(CommitNameChange);
        favouriteButton.onClick.AddListener(ToggleFavourite);
        sellButton.onClick.AddListener(SellAnimal);
    }

    public void Show(string guid, string animalName)
    {
        currentGuid = guid;

        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);

        nameText.text = animalName.Length > MaxNameLength ? animalName[..MaxNameLength] : animalName;
        nameText.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(false);

        ageText.text = FormatAge(entry, data);
        if (sexText != null)
            sexText.text = entry.sex == AnimalSex.Female ? "♀" : "♂";
        quirkText.text = FormatQuirk(entry);

        if (happinessText != null)
        {
            var config = PlayerInventoryManager.Instance.inventoryConfig;
            happinessText.text = FormatHappiness(entry.happinessAmount, config.maxHappiness);
        }

        RefreshFavouriteButton(entry);
        RefreshStorageButton(entry);

        if (sellButton != null && data != null)
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Sell ({data.sellPrice} coins)";

        if (data != null)
            AnimalPortraitRenderer.Instance.Show(data);

        panel.SetActive(true);
    }

    public void Hide()
    {
        AnimalPortraitRenderer.Instance.Clear();
        panel.SetActive(false);
        currentGuid = null;
    }

    private void ToggleFavourite()
    {
        if (currentGuid == null) return;
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == currentGuid);
        if (entry == null) return;

        entry.favourite = !entry.favourite;
        RefreshFavouriteButton(entry);
        DataController.instance.CompleteSave();
    }

    private void RefreshFavouriteButton(PlayerAnimalEntry entry)
    {
        if (favouriteButtonText != null)
            favouriteButtonText.text = entry.favourite ? "♥" : "♡";
    }

    private void SellAnimal()
    {
        if (currentGuid == null) return;

        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == currentGuid);
        if (entry == null) return;

        if (entry.favourite)
        {
            FloatingMessageController.Instance.Show("That one's a favourite — unfavourite them first.");
            return;
        }

        var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
        if (data != null)
            CoinManager.Instance.AddCoins(data.sellPrice);

        string guid = currentGuid;
        Hide();
        PlayerInventoryManager.Instance.RemoveAnimal(guid);
        BarnPanel.Instance.Refresh();
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
            var trimmed = newName.Length > MaxNameLength ? newName[..MaxNameLength] : newName;
            var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == currentGuid);
            if (entry != null)
            {
                entry.customAnimalName = trimmed;
                DataController.instance.CompleteSave();
            }
            newName = trimmed;
        }

        nameText.text = string.IsNullOrWhiteSpace(newName) ? nameText.text : newName;
        nameInputField.gameObject.SetActive(false);
        nameText.gameObject.SetActive(true);
    }

    private void RefreshStorageButton(PlayerAnimalEntry entry)
    {
        if (sendToStorageButtonText == null) return;
        sendToStorageButtonText.text = entry.enclosureType == EnclosureType.Storage
            ? "Bring to Farm"
            : "Send to Barn";
    }

    private void ToggleStorage()
    {
        if (currentGuid == null) return;

        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == currentGuid);
        if (entry == null) return;

        if (entry.enclosureType == EnclosureType.Storage)
        {
            if (!PlayerInventoryManager.Instance.HasOpenActiveSlots())
            {
                FloatingMessageController.Instance.Show("The farm is full — make some room first.");
                return;
            }

            PlayerInventoryManager.Instance.MoveAnimalToFarm(currentGuid);
        }
        else
        {
            PlayerInventoryManager.Instance.MoveAnimalToStorage(currentGuid);
        }

        BarnPanel.Instance?.Refresh();
        Hide();
    }

    private string FormatAge(PlayerAnimalEntry entry, AnimalData data)
    {
        var age = DateTime.Now - new DateTime(entry.timeOfBirth);

        string timeText;
        if (age.TotalDays < 1)      timeText = "Born today";
        else if (age.TotalDays < 2) timeText = "1 day old";
        else                        timeText = $"{(int)age.TotalDays} days old";

        if (data == null) return timeText;

        var config = PlayerInventoryManager.Instance.inventoryConfig;
        var stage = AnimalAgeHelper.GetAgeStage(entry, data, config);
        return $"{timeText} — {FormatAgeStage(stage)}";
    }

    private string FormatAgeStage(AnimalAge stage)
    {
        return stage switch
        {
            AnimalAge.Baby   => "just finding their feet.",
            AnimalAge.Young  => "full of life and curiosity.",
            AnimalAge.Adult  => "in the prime of life.",
            AnimalAge.Senior => "moving a little slower these days.",
            AnimalAge.Old    => "earned every grey hair.",
            _                => ""
        };
    }

    private string FormatHappiness(float happiness, float maxHappiness)
    {
        float ratio = happiness / maxHappiness;
        if (ratio >= 0.8f) return "Absolutely beaming. ♥";
        if (ratio >= 0.6f) return "Content and comfortable.";
        if (ratio >= 0.4f) return "Getting along just fine.";
        if (ratio >= 0.2f) return "Seems a little restless lately...";
        return "Something feels off. Have they been eating?";
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
