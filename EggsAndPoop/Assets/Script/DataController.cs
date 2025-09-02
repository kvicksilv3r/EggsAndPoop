using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public List<PlayerAnimalEntry> playerAnimals = new List<PlayerAnimalEntry>();
    public List<PlayerEggEntry> playerEggs = new List<PlayerEggEntry>();

    public InventoryConfig inventoryConfig;

    public static DataController instance;

    private void Awake()
    {
        instance = this;
    }

    //Egg add
    //Egg remove
}
