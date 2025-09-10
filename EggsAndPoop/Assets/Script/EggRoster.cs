using System.Linq;
using UnityEngine;

public class EggRoster : MonoBehaviour
{
    public EggData[] eggs;

    public static EggRoster Instance;

    private void Awake()
    {
        Instance = this;

        LoadEggs();
    }

    private void LoadEggs()
    {
        eggs = Resources.LoadAll<EggData>("EggData");
    }

    public EggData[] GetData()
    {
        return eggs;
    }

    public EggData GetByType(EggType eggType)
    {
        return eggs.Where(a => a.eggType == eggType).FirstOrDefault();
    }
}
