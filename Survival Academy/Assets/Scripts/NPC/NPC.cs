using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIType
{
    Passive,
    Scared,
    Aggressive
}

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}

public class NPC : MonoBehaviour
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    public AIType aiType;
    private AIState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    // Components
    private NavMeshAgent agent;
    private Animator anim;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        // Get Components
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        // Get player distance
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        switch (aiState)
        {
            case AIState.Idle:
                PassiveUpdate();
                break;
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
            case AIState.Fleeing:
                FleeingUpdate();
                break;
        }
    }

    private void PassiveUpdate()
    {
        // has wandering NPC reached its destination?
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if(aiType == AIType.Aggressive && playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
        else if (aiType == AIType.Scared && playerDistance < detectDistance)
        {
            SetState(AIState.Fleeing);
        }
    }

    private void AttackingUpdate()
    {

    }

    private void FleeingUpdate()
    {

    }

    private void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    private void SetState(AIState newState)
    {
        aiState = newState;

        switch (aiState)
        {
            case AIState.Idle:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = true;
                    break;
                }
            case AIState.Wandering:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = false;
                    break;
                }
            case AIState.Attacking:
                agent.speed = runSpeed;
                break;
            case AIState.Fleeing:
                agent.speed = runSpeed;
                break;
        }
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        while(Vector3.Distance(transform.position, hit.position) < detectDistance && i < 30)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
        }

        return hit.position;
    }
}
