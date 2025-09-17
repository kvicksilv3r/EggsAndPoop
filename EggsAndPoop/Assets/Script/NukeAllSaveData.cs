using System.IO;
using UnityEngine;

public class NukeAllSaveData : MonoBehaviour
{
    public void NukeIt()
    {
        GameManager.Instance.m_NukeSaveData.Invoke();
        ClearPlayerPrefs();
        ClearPersistentData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void ClearPersistentData()
    {
        var path = Application.persistentDataPath;
        string[] filePaths = Directory.GetFiles(path);

        if (filePaths.Length == 0)
        {
            return;
        }

        foreach (string file in filePaths)
        {
            File.Delete(file);
        }
    }

    private void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
