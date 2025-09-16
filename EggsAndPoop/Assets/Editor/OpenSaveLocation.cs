using UnityEditor;
using UnityEngine;

public class OpenSaveLocation : EditorWindow
{
    const string SAVE_DATA_PATH = "/SaveData.txt";

    [MenuItem("EAP/Open Save folder")]
    public static void OpenFolder()
    {
        OpenSaveFolder();
    }

    [MenuItem("EAP/Open SaveData.txt")]
    public static void OpenFile()
    {
        OpenSaveFile();
    }

    private static void OpenSaveFolder()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    private static void OpenSaveFile()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath + SAVE_DATA_PATH);
    }
}
