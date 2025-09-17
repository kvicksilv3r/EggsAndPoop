using System;
using System.Collections;
using UnityEngine;

public class PhysicalAnimal : MonoBehaviour
{
    public PhysicalAnimalData physicalAnimalData;
    public string defaultAnimation = "Idle_A";

    private void Start()
    {
        SetupPosition();
        SetupRotation();

        SetupDefaultAnimation();
        UpdateAnimalData();

        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            UpdateAnimalData();
        }
    }

    public void UpdateAnimalData()
    {
        physicalAnimalData.position = transform.position;
        physicalAnimalData.forward = transform.forward;
    }

    private void SetupDefaultAnimation()
    {
        var animator = GetComponent<Animator>();
        animator.Play(defaultAnimation, 0);
    }

    private void SetupRotation()
    {
        if (physicalAnimalData.forward != Vector3.zero)
        {
            transform.forward = physicalAnimalData.forward;
        }
    }

    private void SetupPosition()
    {
        if (physicalAnimalData.position != Vector3.zero)
        {
            transform.position = physicalAnimalData.position;
        }
    }
}
