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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
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
        SaveGame();
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
        SaveGame();
    }

    private void SaveGame()
    {
        DataIO.Instance.SaveGame();
    }
}
