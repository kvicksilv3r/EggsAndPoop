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
        var savedata = JsonUtility.ToJson(data);
        File.WriteAllText(SavePath(), savedata);
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

    public SaveData Load()
    {
        var rawData = ReadSave();
        var data = JsonUtility.FromJson<SaveData>(rawData);
        return data;
    }

    private string ReadSave()
    {
        return File.ReadAllText(SavePath());
    }
}
