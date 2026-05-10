#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugPanelOpener : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject debugPanel;

    private const int RequiredTaps = 6;
    private const float TapWindow = 2f;

    private int _tapCount;
    private float _lastTapTime;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - _lastTapTime > TapWindow)
            _tapCount = 0;

        _lastTapTime = Time.time;
        _tapCount++;

        if (_tapCount >= RequiredTaps)
        {
            _tapCount = 0;
            debugPanel.SetActive(!debugPanel.activeSelf);
        }
    }
}
#endif
