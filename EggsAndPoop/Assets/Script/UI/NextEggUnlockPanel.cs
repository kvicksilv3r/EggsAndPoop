using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextEggUnlockPanel : MonoBehaviour
{
    public static NextEggUnlockPanel Instance;

    public GameObject panel;
    public GameObject unlockContent;
    public GameObject allUnlockedContent;
    public TextMeshProUGUI costText;
    public Transform conditionsContainer;
    public GameObject conditionRowPrefab;
    public Button unlockButton;

    private EggData _target;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        unlockButton.onClick.AddListener(OnUnlock);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void RefreshUI()
    {
        _target = EggProgressionManager.Instance.GetNextLocked();

        if (_target == null)
        {
            panel.SetActive(true);
            unlockContent.SetActive(false);
            allUnlockedContent.SetActive(true);
            return;
        }

        panel.SetActive(true);
        unlockContent.SetActive(true);
        allUnlockedContent.SetActive(false);
        costText.text = $"{_target.unlockCost} coins";
        BuildConditionRows();
        unlockButton.interactable = EggProgressionManager.Instance.CanPurchase(_target);
    }

    private void BuildConditionRows()
    {
        foreach (Transform child in conditionsContainer)
            Destroy(child.gameObject);

        if (_target.unlockConditions == null) return;

        foreach (var condition in _target.unlockConditions)
        {
            string description = EggProgressionManager.Instance.DescribeCondition(condition, out bool met);
            if (string.IsNullOrEmpty(description)) continue;

            var row = Instantiate(conditionRowPrefab, conditionsContainer);
            var label = row.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = (met ? "[+] " : "[-] ") + description;
        }
    }

    private void OnUnlock()
    {
        if (_target == null) return;
        if (EggProgressionManager.Instance.TryPurchase(_target))
            RefreshUI();
    }
}
