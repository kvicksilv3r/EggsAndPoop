using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public long lastTimeActive;
    public int money = 0;
    public PlayerAnimalEntry[] playerAnimals;
    public PlayerEggEntry[] playerEggs;
    public int extraEggCapacity = 0;
    public EggType[] unlockedEggTypes;
    public string jimmies = "Rustled";
    public long nextEggTime;
    public int farmLevel = 0;
    public int playerLevel = 1;
    public int playerExperience = 0;
    public int fertilizerAmount = 0;
    public int fertilizerCapacity = 0;
}