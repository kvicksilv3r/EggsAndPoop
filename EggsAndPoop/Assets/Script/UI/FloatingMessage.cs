using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingMessage : MonoBehaviour
{
    public TextMeshProUGUI label;
    public float duration = 2f;
    public float floatDistance = 200f;

    public void Show(string message)
    {
        label.text = message;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        var rect = GetComponent<RectTransform>();
        var startPos = rect.anchoredPosition;
        var endPos = startPos + Vector2.up * floatDistance;
        var fadeStartFraction = 0.4f;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            if (t > fadeStartFraction)
            {
                var fadeT = (t - fadeStartFraction) / (1f - fadeStartFraction);
                label.alpha = 1f - fadeT;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
