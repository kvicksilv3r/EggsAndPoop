using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public InventoryConfig inventoryConfig;

    public SaveData saveData;

    private bool ignoreSave = false;

    public static DataController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(CompleteSave);
        GameManager.Instance.m_NukeSaveData.AddListener(IgnoreSave);
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
    }

    public void CheckForSave()
    {
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
        if (ignoreSave)
        {
            return;
        }

        CompleteSave();
    }

    private void IgnoreSave()
    {
        ignoreSave = true;
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
