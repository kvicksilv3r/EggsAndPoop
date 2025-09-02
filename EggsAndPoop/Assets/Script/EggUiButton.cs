using UnityEngine;

public class EggUiButton : MonoBehaviour
{
    public EggData eggData;

    public void SelectEgg()
    {
        EggOpeningController.Instance.InitiateEggOpening(eggData);
    }
}
