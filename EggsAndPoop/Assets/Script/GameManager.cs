using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public UnityEvent m_EggCrackedOpen;
    public UnityEvent m_EggCrackedOpenPost;
    public UnityEvent m_SetupEgg;

    public UnityEvent m_EggContextOpened;
    public UnityEvent m_EggContextClosed;

    public UnityEvent m_AfkDataProcessed;
    public UnityEvent m_SaveDataLoaded;

    public UnityEvent m_NukeSaveData;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }
}
