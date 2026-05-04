using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectFX : MonoBehaviour
{
    public static CollectFX Instance;

    public Canvas overlayCanvas;
    public Texture2D fallbackIcon;

    [SerializeField] private float popDuration = 0.18f;
    [SerializeField] private float flyDuration = 0.55f;
    [SerializeField] private float arcHeight = 200f;
    [SerializeField] private Vector2 iconSize = new Vector2(64, 64);

    private void Awake() => Instance = this;

    public void Play(Vector3 worldOrigin, Texture2D icon, RectTransform target)
    {
        StartCoroutine(FlyRoutine(worldOrigin, icon, target));
    }

    private IEnumerator FlyRoutine(Vector3 worldOrigin, Texture2D icon, RectTransform target)
    {
        var go = new GameObject("CollectFlyingItem", typeof(RectTransform), typeof(RawImage));
        go.transform.SetParent(overlayCanvas.transform, false);

        var rawImage = go.GetComponent<RawImage>();
        rawImage.texture = icon != null ? icon : fallbackIcon;

        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = iconSize;

        Vector2 startPos = WorldToCanvasPos(worldOrigin);
        Vector2 endPos = UIElementToCanvasPos(target);
        rt.anchoredPosition = startPos;
        rt.localScale = Vector3.zero;

        // Pop in
        float t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.one * Mathf.Lerp(0f, 1.3f, t / popDuration);
            yield return null;
        }
        rt.localScale = Vector3.one;

        // Fly along bezier arc, shrinking to nothing at the target
        Vector2 mid = (startPos + endPos) * 0.5f + Vector2.up * arcHeight;
        t = 0;
        while (t < flyDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / flyDuration);
            rt.anchoredPosition = QuadBezier(startPos, mid, endPos, progress);
            rt.localScale = Vector3.one * (1f - progress);
            yield return null;
        }

        Destroy(go);
    }

    private Vector2 WorldToCanvasPos(Vector3 worldPos)
    {
        var cam = Camera.main;
        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)overlayCanvas.transform,
            screenPos,
            overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 localPos
        );
        return localPos;
    }

    private Vector2 UIElementToCanvasPos(RectTransform target)
    {
        var corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector2 screenPos = new Vector2(
            (corners[0].x + corners[2].x) * 0.5f,
            (corners[0].y + corners[2].y) * 0.5f
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)overlayCanvas.transform,
            screenPos,
            overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPos
        );
        return localPos;
    }

    private Vector2 QuadBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }
}
