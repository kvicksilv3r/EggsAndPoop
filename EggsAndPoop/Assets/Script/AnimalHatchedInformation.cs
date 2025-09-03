using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalHatchedInformation : MonoBehaviour
{
    public TextMeshProUGUI rarityTMP;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI descriptionTMP;

    public RarityColorMap rarityColorMap;

    private void Start()
    {
        GameManager.Instance.m_SetupEgg.AddListener(Hide);
        GameManager.Instance.m_EggContextOpened.AddListener(Hide);
        GameManager.Instance.m_EggContextClosed.AddListener(Hide);
    }

    public void DisplayInfo(AnimalData animalData)
    {
        nameTMP.text = animalData.animalName;
        descriptionTMP.text = animalData.description;

        var color = rarityColorMap.colorMap.Where(a => a.rarity == animalData.rarity).FirstOrDefault().rarityColor;
        var rarityString = System.Enum.GetName(typeof(AnimalRarity), animalData.rarity);

        rarityTMP.text = $"<color=#{color.ToHexString()}>{rarityString}</color>";

        Display();
    }

    private void Display()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
