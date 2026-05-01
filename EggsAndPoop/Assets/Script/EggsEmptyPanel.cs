using TMPro;
using UnityEngine;

public class EggsEmptyPanel : MonoBehaviour
{
    public TextMeshProUGUI nextEggInTMP;
    public string baseText = "Next egg in";

    void Update()
    {
        if (EggSlotManager.Instance == null) return;

        nextEggInTMP.text = $"{baseText} {EggSlotManager.Instance.TimeUntilNextEgg()}";
    }
}
