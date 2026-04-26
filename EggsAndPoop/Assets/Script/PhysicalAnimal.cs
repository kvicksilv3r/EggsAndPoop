using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PhysicalAnimal : MonoBehaviour
{
    public PhysicalAnimalData physicalAnimalData;
    public AnimalData animalData;
    public string animalGuid;
    public string defaultAnimation = "Idle_A";
    public Transform visualHolster;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public void Initialize(AnimalData data, PhysicalAnimalData physicalData)
    {
        animalData = data;
        physicalAnimalData = physicalData;

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        SetupHierarchy();
        SetupNavAgent();
        SetupPosition();
        SetupRotation();
        SetupDefaultAnimation();
        UpdateAnimalData();

        StartCoroutine(Loop());

        if (animalData.poopRate > 0)
            StartCoroutine(PoopLoop());
    }

    private IEnumerator PoopLoop()
    {
        var interval = 3600f / animalData.poopRate;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            PoopManager.Instance.AddPoop(1);
        }
    }

    private void SetupHierarchy()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Animal"))
            {
                child.parent = visualHolster;
                break;
            }
        }
    }

    private void SetupNavAgent()
    {
        navMeshAgent.acceleration = animalData.acceleration;
        navMeshAgent.speed = animalData.movementSpeed;
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
            navMeshAgent.Warp(physicalAnimalData.position);
        }
    }
}
