using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public long lastTimeActive;
    public int money = 0;
    public PlayerAnimalEntry[] playerAnimals = new PlayerAnimalEntry[0];
    public PlayerEggEntry[] playerEggs = new PlayerEggEntry[0];
    public int extraEggCapacity = 0;
    public EggType[] unlockedEggTypes = new EggType[0];
    public string jimmies = "Rustled";
    public long nextEggTime;
    public int farmLevel = 0;
    public int playerLevel = 1;
    public int playerExperience = 0;
    public int fertilizerAmount = 0;
    public int fertilizerCapacity = 0;
    public int foodAmount = 0;
    public int foodCapacity = 0;
    public int poopAmount = 0;
    public int poopCapacity = 0;
}