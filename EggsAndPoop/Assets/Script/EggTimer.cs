using UnityEngine;

// Egg production is now handled by EnclosureManager.
// This class is kept as an empty shell to avoid breaking scene references.
public class EggTimer : MonoBehaviour
{
    public static EggTimer Instance;

    private void Awake()
    {
        Instance = this;
    }
}
