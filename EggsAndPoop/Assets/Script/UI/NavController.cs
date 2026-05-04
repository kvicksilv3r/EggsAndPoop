using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NavController : MonoBehaviour
{
    public static NavController Instance { get; private set; }

    [Header("Screen Panels")]
    public RectTransform farmPanel;
    public RectTransform eggsPanel;
    public RectTransform upgradesPanel;
    public RectTransform barnPanel;

    [Header("Egg Screen")]
    public Camera eggCamera;
    public RawImage eggRawImage;
    public Canvas eggOverlayCanvas;

    [Header("Navigation")]
    public NavBar navBar;
    public float slideDuration = 0.3f;

    public const int TAB_FARM = 0;
    public const int TAB_EGGS = 1;
    public const int TAB_UPGRADES = 2;
    public const int TAB_BARN = 3;

    private RectTransform[] panels;
    private int currentIndex = TAB_FARM;
    private bool isTransitioning = false;
    private float panelWidth;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        panels = new[] { farmPanel, eggsPanel, upgradesPanel, barnPanel };
    }

    private void Start()
    {
        panelWidth = farmPanel.parent.GetComponent<RectTransform>().rect.width;
        SetupEggRenderTexture();
        InitialisePanels();
        navBar.SetActiveTab(currentIndex);
    }

    private void SetupEggRenderTexture()
    {
        var rt = new RenderTexture(Screen.width, Screen.height, 24);
        eggCamera.targetTexture = rt;
        eggRawImage.texture = rt;
    }

    private void InitialisePanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].anchoredPosition = new Vector2(i == TAB_FARM ? 0f : panelWidth, 0f);
            panels[i].gameObject.SetActive(i == TAB_FARM);
        }

        if (eggOverlayCanvas != null)
            eggOverlayCanvas.enabled = false;
    }

    public void Navigate(int targetIndex)
    {
        if (isTransitioning || targetIndex == currentIndex) return;
        StartCoroutine(Slide(currentIndex, targetIndex));
    }

    private IEnumerator Slide(int from, int to)
    {
        isTransitioning = true;

        if (from == TAB_EGGS)
        {
            if (eggOverlayCanvas != null) eggOverlayCanvas.enabled = false;
            GameManager.Instance.m_EggContextClosed.Invoke();
        }

        float dir = to > from ? 1f : -1f;

        if (to != TAB_FARM)
        {
            panels[to].anchoredPosition = new Vector2(dir * panelWidth, 0f);
            panels[to].gameObject.SetActive(true);
        }

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Smooth(Mathf.Clamp01(elapsed / slideDuration));

            if (from != TAB_FARM)
                panels[from].anchoredPosition = new Vector2(Mathf.Lerp(0f, -dir * panelWidth, t), 0f);

            if (to != TAB_FARM)
                panels[to].anchoredPosition = new Vector2(Mathf.Lerp(dir * panelWidth, 0f, t), 0f);

            yield return null;
        }

        if (from != TAB_FARM)
        {
            panels[from].anchoredPosition = new Vector2(-dir * panelWidth, 0f);
            panels[from].gameObject.SetActive(false);
        }

        if (to != TAB_FARM)
            panels[to].anchoredPosition = Vector2.zero;

        currentIndex = to;
        isTransitioning = false;

        navBar.SetActiveTab(currentIndex);

        if (to == TAB_EGGS)
        {
            if (eggOverlayCanvas != null)
            {
                var parent = eggOverlayCanvas.transform.parent?.gameObject;
                if (parent != null && !parent.activeSelf) parent.SetActive(true);
                eggOverlayCanvas.enabled = true;
            }
            GameManager.Instance.m_EggContextOpened.Invoke();
        }

        if (to == TAB_UPGRADES && FarmUpgradePanel.Instance != null)
            FarmUpgradePanel.Instance.RefreshUI();
    }

    private float Smooth(float t) => t * t * (3f - 2f * t);

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isTransitioning && currentIndex != TAB_FARM)
            Navigate(TAB_FARM);
    }
}
