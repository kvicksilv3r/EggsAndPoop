using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarnAnimalEntry : MonoBehaviour
{
    public RawImage animalImage;
    public TextMeshProUGUI nameText;

    private string guid;

    public void Setup(PlayerAnimalEntry entry)
    {
        guid = entry.guid;
        nameText.text = entry.customAnimalName;

        var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
        if (data != null && animalImage != null)
        {
            animalImage.texture = data.icon;
            animalImage.enabled = data.icon != null;
        }
    }

    public void OnClick()
    {
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        if (entry != null)
            AnimalInfoPanel.Instance.Show(guid, entry.customAnimalName);
    }
}
