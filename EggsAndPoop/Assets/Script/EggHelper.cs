using DG.Tweening;
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

    public static EggHelper Instance;

    public float eggSpawnOffset;
    public float eggSpawnTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.m_SetupEgg.AddListener(SetupEgg);
    }

    public void SetupEgg()
    {
        if (EggExists())
        {
            DestroyCurrentEgg();
        }

        var eggdata = EggOpeningController.Instance.GetEgg();

        var egg = Instantiate(eggdata.eggPrefab, eggSceneRoot.transform);
        egg.GetComponent<EggCracking>().SetCamera(eggCamera);

        egg.transform.localPosition = position + Vector3.up * eggSpawnOffset;
        egg.transform.rotation = Quaternion.Euler(rotation);
        egg.transform.localScale = scale;

        activeEgg = egg;

        egg.transform.DOMove(eggSceneRoot.transform.position + position, eggSpawnTime);
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
