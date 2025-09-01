using System;
using UnityEngine;

public class EggHelper : MonoBehaviour
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public GameObject crackingEgg;

    public GameObject eggSceneRoot;

    public GameObject activeEgg;

    public Camera eggCamera;

    void Start()
    {

    }

    public void SetupEgg()
    {
        if (EggExists())
        {
            DestroyCurrentEgg();
        }

        var egg = Instantiate(crackingEgg, eggSceneRoot.transform);
        egg.GetComponent<EggCracking>().SetCamera(eggCamera);

        egg.transform.localPosition = position;
        egg.transform.rotation = Quaternion.Euler(rotation);
        egg.transform.localScale = scale;

        activeEgg = egg;
    }

    private void DestroyCurrentEgg()
    {
        GameObject.DestroyImmediate(activeEgg);
        activeEgg = null;
    }

    private bool EggExists()
    {
        return activeEgg != null;
    }
}
