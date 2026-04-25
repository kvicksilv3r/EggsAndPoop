using System;

[System.Serializable]
public class PlayerAnimalEntry
{
    public AnimalEnum animalIdentifier;
    public string customAnimalName;
    public bool favourite = false;
    public EnclosureType enclosureType = EnclosureType.Pasture;
    public bool isInStorage = false; // legacy — migrated to enclosureType in LoadInventory
    public long timeOfBirth;
    public string guid;
    public int quirkId = -1;
    public PhysicalAnimalData physicalAnimalData;
}
