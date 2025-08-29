using UnityEngine;
using UnityEngine.SceneManagement;

public class UglyReset : MonoBehaviour
{
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
