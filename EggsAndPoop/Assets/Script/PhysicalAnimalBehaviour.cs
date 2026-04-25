using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PhysicalAnimalBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public string idleAnimation = "Idle_A";
    public string eatAnimation = "Eat";
    public string walkAnimation = "Walk";
    public string rareIdleAnimation = "Idle_B";
    public string flyingAnimation = "Fly";
    public float minActionTime = 4;
    public float maxActionTime = 8;
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

        if (Random.Range(0, 100) < specialIdleChance)
        {
            animator.Play(rareIdleAnimation, 0);
        }
        else
        {
            animator.Play(idleAnimation, 0);
        }
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
        animator.Play(flyingAnimation, 0);
        visualHolster.transform.localPosition = Vector3.zero + Vector3.up * draggedTransformOffset;
    }

    public void StopDragged()
    {
        visualHolster.transform.localPosition = Vector3.zero;
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
            yield return new WaitForSeconds(Random.Range(minActionTime, maxActionTime));
        }
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var animal = GetComponent<PhysicalAnimal>();
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == animal.animalGuid);
        if (entry == null) return;

        AnimalInfoPanel.Instance.Show(animal.animalGuid, entry.customAnimalName);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        AnimalInfoPanel.Instance.Hide();
        SetState(AnimalBehaviourState.Dragged);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("End drag");
        StopDragged();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.pointerCurrentRaycast.worldPosition;
    }
}
