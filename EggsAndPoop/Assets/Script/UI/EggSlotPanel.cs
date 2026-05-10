using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EggSlotPanel : MonoBehaviour
{
    [System.Serializable]
    public class SlotRow
    {
        public TextMeshProUGUI labelText;
        public Image fillBar;
        public TextMeshProUGUI progressText;
        public RawImage preferredEggIcon;       // optional — shows current preferred egg icon
        public TextMeshProUGUI preferredEggLabel; // optional — shows egg name or "Random"
        public Button preferenceButton;          // optional — opens NestPreferencePanel for this slot
    }

    // 4 rows: indices 0-2 are regular slots, index 3 is the carton
    public SlotRow[] rows;

    private static readonly string[] Labels = { "Slot 1", "Slot 2", "Slot 3", "Carton" };

    private void Start()
    {
        // Wire preference buttons once at startup
        for (int i = 0; i < 3 && rows != null && i < rows.Length; i++)
        {
            int captured = i;
            rows[i]?.preferenceButton?.onClick.AddListener(() => OpenPreferencePanel(captured));
        }
    }

    private void Update()
    {
        if (EggSlotManager.Instance == null || rows == null || rows.Length < 4) return;

        for (int i = 0; i < 4; i++)
        {
            var row = rows[i];
            if (row == null) continue;

            float cost = EggSlotManager.Instance.GetSlotCostForIndex(i);
            float progress = EggSlotManager.Instance.GetSlotProgress(i);
            bool occupied = EggNestManager.Instance != null && EggNestManager.Instance.IsNestOccupied(i);

            if (row.labelText != null)
                row.labelText.text = Labels[i];

            if (row.fillBar != null)
                row.fillBar.fillAmount = occupied ? 1f : (cost > 0f ? progress / cost : 0f);

            if (row.progressText != null)
                row.progressText.text = occupied ? "Ready!" : EggSlotManager.Instance.TimeUntilNextEgg(i);

            // Preferred egg display (slots 0-2 only)
            if (i < 3)
                RefreshPreferenceDisplay(row, i);
        }
    }

    private void RefreshPreferenceDisplay(SlotRow row, int slotIndex)
    {
        var preferred = EggSlotManager.Instance.GetNestPreference(slotIndex);
        bool isRandom = preferred == EggType.Unknown;

        EggData eggData = isRandom ? null : EggRoster.Instance.GetByType(preferred);

        if (row.preferredEggIcon != null)
        {
            row.preferredEggIcon.texture = eggData?.eggIcon;
            row.preferredEggIcon.enabled = eggData != null;
        }

        if (row.preferredEggLabel != null)
            row.preferredEggLabel.text = isRandom ? "Random" : eggData?.eggName ?? "?";
    }

    private void OpenPreferencePanel(int slotIndex)
    {
        NestPreferencePanel.Instance?.Open(slotIndex);
    }
}
