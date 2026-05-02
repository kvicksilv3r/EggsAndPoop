using System;

[System.Serializable]
public class PlayerAnimalEntry
{
    public AnimalEnum animalIdentifier;
    public string customAnimalName;
    public bool favourite = false;
    public EnclosureType enclosureType = EnclosureType.Pasture;
    public long timeOfBirth;
    public string guid;
    public int quirkId = -1;
    public AnimalSex sex;
    public PhysicalAnimalData physicalAnimalData;
    public float happinessAmount = 0f;
    public bool oldAgeNotified = false;
}
