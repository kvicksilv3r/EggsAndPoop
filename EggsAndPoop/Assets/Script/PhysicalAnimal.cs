using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(NavMeshAgent))]
public class PhysicalAnimal : MonoBehaviour
{
    public PhysicalAnimalData physicalAnimalData;
    public AnimalData animalData;
    public string defaultAnimation = "Idle_A";
    public string eatAnimation = "Eat";
    public string walkAnimation = "Walk";
    public NavMeshAgent navMeshAgent;
    public float timeBetweenFrolicActions = 5;
    public int eatChance = 10;
    public int chillChance = 50;
    public Animator animator;
    public LayerMask layermask;
    public float dist = 12;

    public int specialIdleChance = 5;

    private void Start()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        SetupNavAgent();

        SetupPosition();
        SetupRotation();

        SetupDefaultAnimation();
        UpdateAnimalData();

        StartCoroutine(Loop());
        StartCoroutine(Frolicking());
    }

    private void SetupNavAgent()
    {
        navMeshAgent.acceleration = animalData.acceleration;
        navMeshAgent.speed = animalData.movementSpeed;
    }

    IEnumerator Frolicking()
    {
        while (true)
        {
            var rng = Random.Range(0, 100);

            if (rng <= eatChance)
            {
                navMeshAgent.isStopped = true;
                animator.Play(eatAnimation, 0);

            }
            else if (rng <= chillChance)
            {
                navMeshAgent.isStopped = true;
                animator.Play(defaultAnimation, 0);
            }
            else
            {
                Vector3 targetLocation = GetRandomPoint();
                navMeshAgent.SetDestination(targetLocation);
                navMeshAgent.isStopped = false;
                animator.Play(walkAnimation, 0);
            }
            yield return new WaitForSeconds(timeBetweenFrolicActions);
        }
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
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
            transform.position = physicalAnimalData.position;
        }
    }
}
