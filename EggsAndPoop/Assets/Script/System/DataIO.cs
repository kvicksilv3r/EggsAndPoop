using System;
using System.IO;
using UnityEngine;

public class DataIO : MonoBehaviour
{
    const string SAVE_DATA_PATH = "/SaveData.txt";
    public static DataIO Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SaveGame(SaveData dataToBeSaved)
    {
        WriteSaveData(dataToBeSaved);
    }

    public void WriteSaveData(SaveData data)
    {
        var json = JsonUtility.ToJson(data);
        var tempPath = SavePath() + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Copy(tempPath, SavePath(), overwrite: true);
        File.Delete(tempPath);
        print("SAVEDATA SAVED");
    }

    public bool HasSave()
    {
        return File.Exists(SavePath());
    }

    private string SavePath()
    {
        return Application.persistentDataPath + SAVE_DATA_PATH;
    }

    // Returns null if the file is missing, empty, or corrupt.
    public SaveData Load()
    {
        try
        {
            var raw = File.ReadAllText(SavePath());
            return JsonUtility.FromJson<SaveData>(raw);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save data could not be read, starting fresh. ({e.Message})");
            return null;
        }
    }
}
