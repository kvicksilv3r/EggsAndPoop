using System.Collections;
using UnityEngine;

public class AnimalPortraitRenderer : MonoBehaviour
{
    public static AnimalPortraitRenderer Instance;

    public Transform animalAnchor;
    public int portraitLayer = 8;

    private const int ClicksToSpin = 5;
    private const float SpinDurationForEyes = 5f;

    private GameObject _current;
    private Animator _animator;
    private int _clickCount;
    private Coroutine _spinWatcher;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(AnimalData data)
    {
        Clear();

        if (data.prefab == null) return;

        _current = Instantiate(data.prefab, animalAnchor.position, animalAnchor.rotation);
        SetLayerRecursively(_current, portraitLayer);

        _animator = _current.GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.Play("Idle_A", 0);
            _animator.Play("Eyes_Happy", 1);
        }

        _clickCount = 0;
    }

    public void Clear()
    {
        if (_spinWatcher != null)
        {
            StopCoroutine(_spinWatcher);
            _spinWatcher = null;
        }

        if (_current != null)
        {
            Destroy(_current);
            _current = null;
        }

        _animator = null;
        _clickCount = 0;
    }

    public void OnPortraitClicked()
    {
        if (_animator == null) return;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Spin"))
        {
            StopSpin();
            return;
        }

        _clickCount++;
        if (_clickCount < ClicksToSpin) return;

        _clickCount = 0;
        _animator.Play("Spin", 0);

        if (_spinWatcher != null)
            StopCoroutine(_spinWatcher);
        _spinWatcher = StartCoroutine(WatchSpin());
    }

    private void StopSpin()
    {
        if (_spinWatcher != null)
        {
            StopCoroutine(_spinWatcher);
            _spinWatcher = null;
        }

        _clickCount = 0;
        _animator.Play("Idle_A", 0);
    }

    private IEnumerator WatchSpin()
    {
        // Wait a frame for the animator to transition into Spin before checking
        yield return null;

        float elapsed = 0f;

        while (elapsed < SpinDurationForEyes)
        {
            if (_animator == null) yield break;

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Spin")) yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_animator != null)
            _animator.Play("Eyes_Spin", 1);
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
