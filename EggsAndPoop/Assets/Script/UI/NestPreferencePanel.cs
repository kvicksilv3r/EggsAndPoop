using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NestPreferencePanel : MonoBehaviour
{
    public static NestPreferencePanel Instance;

    public GameObject panel;
    public Transform buttonContainer;
    public GameObject eggChoiceButtonPrefab;  // Button with RawImage icon + TextMeshProUGUI label

    private int _targetSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(int slotIndex)
    {
        _targetSlot = slotIndex;
        BuildButtons();
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void BuildButtons()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        var unlocked = PlayerInventoryManager.Instance.unlockedEggTypes;
        var ordered = EggProgressionManager.Instance.GetOrderedEggs();

        foreach (var eggData in ordered)
        {
            if (unlocked.Contains(eggData.eggType))
                SpawnButton(eggData);
        }
    }

    private void SpawnButton(EggData eggData)
    {
        var go = Instantiate(eggChoiceButtonPrefab, buttonContainer);
        var btn = go.GetComponent<Button>();
        var icon = go.GetComponentInChildren<RawImage>();
        var label = go.GetComponentInChildren<TextMeshProUGUI>();

        bool isRandom = eggData == null;

        if (icon != null)
            icon.texture = isRandom ? null : (eggData.nestPickerIcon != null ? eggData.nestPickerIcon : eggData.eggIcon);

        if (label != null)
            label.text = isRandom ? "Random" : eggData.eggName;

        var capturedType = isRandom ? EggType.Unknown : eggData.eggType;
        btn?.onClick.AddListener(() =>
        {
            EggSlotManager.Instance.SetNestPreference(_targetSlot, capturedType);
            Close();
        });
    }
}
