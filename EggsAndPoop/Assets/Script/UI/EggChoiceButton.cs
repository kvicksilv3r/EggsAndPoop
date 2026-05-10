using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EggChoiceButton : MonoBehaviour
{
    public RawImage icon;
    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI multiplierLabel;

    public void Setup(EggData eggData)
    {
        icon.texture = eggData.nestPickerIcon != null ? eggData.nestPickerIcon : eggData.eggIcon;
        nameLabel.text = eggData.eggName;
        multiplierLabel.text = $"{eggData.nestLoveCostMultiplier:0.##}×";
    }
}
