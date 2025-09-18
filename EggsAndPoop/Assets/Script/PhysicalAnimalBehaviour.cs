using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PhysicalAnimalBehaviour : MonoBehaviour
{
    public string idleAnimation = "Idle_A";
    public string eatAnimation = "Eat";
    public string walkAnimation = "Walk";
    public float timeBetweenFrolicActions = 5;
    public int eatChance = 10;
    public int chillChance = 50;
    public LayerMask layermask;
    public float dist = 12;
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public AnimalBehaviourState currentState;
    public AnimalBehaviourState lastState;
    public int specialIdleChance = 5;
    public Transform visualHolster;
    public float draggedTransformOffset = 2f;
    public float distanceToTargetAsStopped = 0.1f;

    private void Start()
    {
        SetupComponents();

        StartCoroutine(Frolicking());
    }

    private void SetupComponents()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void SetState(AnimalBehaviourState state)
    {
        lastState = currentState;
        currentState = state;

        switch (currentState)
        {
            case AnimalBehaviourState.Idle:
                StartIdling();
                break;
            case AnimalBehaviourState.Walking:
                StartWalking();
                break;
            case AnimalBehaviourState.Eating:
                StartEating();
                break;
            case AnimalBehaviourState.Afraid:
                break;
            case AnimalBehaviourState.Swimming:
                break;
            case AnimalBehaviourState.Flying:
                break;
            case AnimalBehaviourState.Dead:
                break;
            case AnimalBehaviourState.Dragged:
                StartDragged();
                break;
        }
    }

    private void StartEating()
    {
        navMeshAgent.isStopped = true;
        animator.Play(eatAnimation, 0);
    }

    private void StartIdling()
    {
        navMeshAgent.isStopped = true;
        animator.Play(idleAnimation, 0);
    }

    private void StartWalking()
    {
        Vector3 targetLocation = GetRandomPoint();
        navMeshAgent.SetDestination(targetLocation);
        navMeshAgent.isStopped = false;
        animator.Play(walkAnimation, 0);
    }

    private void StartDragged()
    {
        StopAllCoroutines();
        navMeshAgent.isStopped = true;
        visualHolster.transform.position = Vector3.zero + Vector3.up * draggedTransformOffset;
    }

    public void StopDragged()
    {
        visualHolster.transform.position = Vector3.zero;
        EnterRandomState();
        StartCoroutine(Frolicking());
    }

    private void Update()
    {
        if (currentState == AnimalBehaviourState.Walking)
        {
            if (navMeshAgent.remainingDistance < distanceToTargetAsStopped)
            {
                SetState(AnimalBehaviourState.Idle);
            }
        }
    }

    private void EnterRandomState()
    {
        var states = new AnimalBehaviourState[] { AnimalBehaviourState.Idle, AnimalBehaviourState.Walking, AnimalBehaviourState.Eating };
        SetState(states[Random.Range(0, states.Length)]);
    }

    IEnumerator Frolicking()
    {
        while (true)
        {
            var rng = Random.Range(0, 100);

            if (rng <= eatChance)
            {
                SetState(AnimalBehaviourState.Eating);

            }
            else if (rng <= chillChance)
            {
                SetState(AnimalBehaviourState.Idle);
            }
            else
            {
                SetState(AnimalBehaviourState.Walking);
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
}
