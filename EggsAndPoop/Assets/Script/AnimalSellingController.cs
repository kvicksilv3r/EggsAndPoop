using System;
using UnityEngine;

public class AnimalSellingController : MonoBehaviour
{
    public static AnimalSellingController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterAnimal(PhysicalAnimal animal)
    {

    }

    public void RegisterAnimal(Guid animalGuid)
    {

    }

    public void SellAnimal()
    {

    }
}
