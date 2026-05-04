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
    }

    // 4 rows: indices 0-2 are regular slots, index 3 is the carton
    public SlotRow[] rows;

    private static readonly string[] Labels = { "Slot 1", "Slot 2", "Slot 3", "Carton" };

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
                row.progressText.text = occupied ? "Ready!" : $"{Mathf.FloorToInt(progress)} / {Mathf.FloorToInt(cost)}";
        }
    }
}
