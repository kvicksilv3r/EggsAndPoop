using System;

[System.Serializable]
public class PlayerAnimalEntry
{
    public AnimalEnum animalIdentifier;
    public string customAnimalName;
    public bool favourite = false;
    public long timeOfBirth;
    public string guid;
    public PhysicalAnimalData physicalAnimalData;
}
