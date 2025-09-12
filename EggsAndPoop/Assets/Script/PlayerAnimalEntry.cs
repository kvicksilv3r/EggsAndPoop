using System;
using System.Numerics;

[System.Serializable]
public class PlayerAnimalEntry
{
    public AnimalData animalData;
    public string customAnimalName;
    public bool favourite = false;
    public PhysicalAnimalData physicalAnimalData;
    public DateTime timeOfBirth;
}
