using TMPro;
using UnityEngine;

public class EggsEmptyPanel : MonoBehaviour
{
    public TextMeshProUGUI nextEggInTMP;
    public string baseText = "Next egg in";

    void Update()
    {
        var untilEgg = EggTimer.Instance.StringUntilNextTime();

        nextEggInTMP.text = $"{baseText} {untilEgg}";
    }
}
