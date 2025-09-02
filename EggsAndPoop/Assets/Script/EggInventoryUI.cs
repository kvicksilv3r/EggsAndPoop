using UnityEngine;

public class EggInventoryUI : MonoBehaviour
{
    public EggData egg;

    public void InitializeEggOpening()
    {
        EggOpeningController.Instance.InitiateEggOpening(egg);
    }
}
