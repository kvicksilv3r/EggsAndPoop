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
    public int specialIdleChance = 5;
    public Transform visualHolster;
    public float draggedTransformOffset = 2f;
    public float distanceToTargetAsStopped = 0.1f;

    private Vector3 _dragStartPosition;

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
        _dragStartPosition = transform.position;
        StopAllCoroutines();
        navMeshAgent.enabled = false;
        animator.Play(flyingAnimation, 0);
        visualHolster.transform.localPosition = Vector3.up * draggedTransformOffset;
    }

    public void StopDragged(bool returnToStart = false)
    {
        visualHolster.transform.localPosition = Vector3.zero;

        Vector3 landingPosition = returnToStart || !NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas)
            ? _dragStartPosition
            : hit.position;

        transform.position = landingPosition;
        navMeshAgent.enabled = true;
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
        Vector3 randDirection = transform.position + Random.insideUnitSphere * dist;

        if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask))
            return navHit.position;

        return transform.position;
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
        var enclosure = FindEnclosureAtPosition(transform.position);
        var animal = GetComponent<PhysicalAnimal>();
        EnclosureType targetType = enclosure != null ? enclosure.enclosureType : EnclosureType.Pasture;

        bool assigned = EnclosureManager.Instance.AssignToEnclosure(animal.animalGuid, targetType);

        if (targetType == EnclosureType.Storage)
            return; // animal GameObject is destroyed by AssignToEnclosure, don't touch it after

        StopDragged(returnToStart: !assigned);
    }

    private EnclosureVolume FindEnclosureAtPosition(Vector3 position)
    {
        var hits = Physics.OverlapSphere(position, 0.5f);
        foreach (var hit in hits)
        {
            var volume = hit.GetComponent<EnclosureVolume>();
            if (volume != null) return volume;
        }
        return null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.pointerCurrentRaycast.worldPosition;
    }
}
