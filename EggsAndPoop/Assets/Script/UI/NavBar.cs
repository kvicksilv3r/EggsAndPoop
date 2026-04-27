using UnityEngine;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    public Button farmButton;
    public Button eggsButton;
    public Button upgradesButton;
    public Button barnButton;

    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.45f);

    private Button[] buttons;

    private void Awake()
    {
        buttons = new[] { farmButton, eggsButton, upgradesButton, barnButton };

        farmButton.onClick.AddListener(() => NavController.Instance.Navigate(NavController.TAB_FARM));
        eggsButton.onClick.AddListener(() => NavController.Instance.Navigate(NavController.TAB_EGGS));
        upgradesButton.onClick.AddListener(() => NavController.Instance.Navigate(NavController.TAB_UPGRADES));
        barnButton.onClick.AddListener(() => NavController.Instance.Navigate(NavController.TAB_BARN));
    }

    public void SetActiveTab(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.color = i == index ? activeColor : inactiveColor;
        }
    }
}
