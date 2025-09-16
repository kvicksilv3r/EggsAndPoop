using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public InventoryConfig inventoryConfig;

    public SaveData saveData;

    public static DataController instance;

    public bool ignoreLoad = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(CheckForSave());
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(CompleteSave);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CompleteSave();
        }
    }

    private void LoadGame()
    {
        saveData = DataIO.Instance.Load();
        GameManager.Instance.m_SaveDataLoaded.Invoke();
    }

    public IEnumerator CheckForSave()
    {
        yield return new WaitForSeconds(0.1f);

        if (ignoreLoad)
        {
            yield return null;
        }

        if (DataIO.Instance.HasSave())
        {
            LoadGame();
        }

        else
        {
            SetupFirstSave();
        }
    }

    private void SetupFirstSave()
    {
        saveData = new SaveData();
        EggTimer.Instance.SetNextEggTime();
        CompleteSave();
    }

    public SaveData GetDataToBeSaved()
    {
        return GenerateSaveData();
    }

    public SaveData GetData()
    {
        return saveData;
    }

    private SaveData GenerateSaveData()
    {
        saveData.lastTimeActive = DateTime.Now.Ticks;
        PlayerInventoryManager.Instance.ModifySaveData(ref saveData);
        EggTimer.Instance.ModifySaveData(ref saveData);
        return saveData;
    }

    private void OnApplicationQuit()
    {
        CompleteSave();
    }

    public void CompleteSave()
    {
        DataIO.Instance.SaveGame(GenerateSaveData());
    }

    private void SaveGame()
    {
        saveData.lastTimeActive = DateTime.Now.Ticks;
        DataIO.Instance.SaveGame(saveData);
    }
}
