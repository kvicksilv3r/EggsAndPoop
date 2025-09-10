using UnityEngine;

public class EggTimer : MonoBehaviour
{
    public void CalculateEggs()
    {
        var lastTimeActive = DataController.instance.GetDataToBeSaved().lastTimeActive;
    }
}
