using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EggUiButton : MonoBehaviour
{
    public EggData eggData;
    public RawImage eggIcon;
    public TextMeshProUGUI eggNameTMP;
    public TextMeshProUGUI eggAmountTMP;

    public void SelectEgg()
    {
        EggOpeningController.Instance.InitiateEggOpening(eggData);
    }

    public void SetupButton(PlayerEggEntry eggEntry)
    {
        this.eggData = eggEntry.eggData;
        ConstructButton(eggEntry.eggAmount);
    }

    private void ConstructButton(int eggAmount)
    {
        eggIcon.texture = eggData.eggIcon;
        eggNameTMP.text = eggData.eggName;
        eggAmountTMP.text = eggAmount.ToString();
    }
}
