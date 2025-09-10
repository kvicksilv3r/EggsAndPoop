using System;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public InventoryConfig inventoryConfig;

    public SaveData saveData;

    public static DataController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CheckForSave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TryLoadGame();
        }
    }

    private void TryLoadGame()
    {
        saveData = DataIO.Instance.Load();
        GameManager.Instance.m_SaveDataLoaded.Invoke();
    }

    public void CheckForSave()
    {
        if (saveData == null)
        {
            if (DataIO.Instance.HasSave())
            {
                saveData = DataIO.Instance.Load();
            }

            else
            {
                saveData = new SaveData();
            }
        }
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
        PlayerInventoryManager.Instance.ModifySaveData(ref saveData);
        return saveData;
    }


}
