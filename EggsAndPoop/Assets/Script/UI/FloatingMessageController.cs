using UnityEngine;

public class FloatingMessageController : MonoBehaviour
{
    public static FloatingMessageController Instance { get; private set; }

    public GameObject messagePrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string message)
    {
        var obj = Instantiate(messagePrefab, transform);
        obj.GetComponent<FloatingMessage>().Show(message);
    }
}
