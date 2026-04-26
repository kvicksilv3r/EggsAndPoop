using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalPortraitClickReceiver : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AnimalPortraitRenderer.Instance.OnPortraitClicked();
    }
}
