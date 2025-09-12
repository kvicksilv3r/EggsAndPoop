using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public long lastTimeActive;
    public int money;
    public PlayerAnimalEntry[] playerAnimals;
    public PlayerEggEntry[] playerEggs;
    public int extraEggCapacity = 0;
    public EggType[] unlockedEggTypes;
    public string jimmies = "Rustled";
    public long nextEggTime;
}