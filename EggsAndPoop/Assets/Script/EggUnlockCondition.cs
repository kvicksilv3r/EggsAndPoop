using System;

public enum EggUnlockConditionType
{
    TotalAnimals,
    FarmLevel,
    AnimalsOfFamily,
    EggTypeUnlocked,
}

[Serializable]
public class EggUnlockCondition
{
    public EggUnlockConditionType type;
    public int requiredAmount;
    public AnimalFamily requiredFamily;
    public EggType requiredEggType;
}
