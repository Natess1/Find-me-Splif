using UnityEngine;
using UnityEngine.AI;
using FMS.Utils;
using UnityEngine.InputSystem.XR.Haptics;
using System;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] private State startingState;

    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;
    [SerializeField] private float attackingDistance = 2f;
    [SerializeField] private float attackRate = 2f;

    [SerializeField] private bool isChaisingEnemy = false;
    [SerializeField] private bool isAttackingEnemy = false;

    private NavMeshAgent navMeshAgent;
    private State state;
    private Vector3 roamPos;
    private Vector3 startingPos;
    private Vector3 lastPos;
    private float roamingTime;
    private float roamingSpeed;
    private float chasingSpeed;

    private float nextAttackTime = 0f;
    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;

    public event EventHandler OnEnemyAttack;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        state = startingState;

        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public bool IsRunning => navMeshAgent.velocity != Vector3.zero;
    
    public float GetRoamingSpeed()
    {
        return navMeshAgent.speed / roamingSpeed;
    }

    public void SetDeathState()
    {
        navMeshAgent.ResetPath();
        state = State.Death;
    }

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void StateHandler()
    {
        switch (state)
        {
            case State.Roaming:
                roamingTime -= Time.deltaTime;
                if (roamingTime < 0)
                {
                    Roaming();
                    roamingTime = roamingTimerMax;
                }
                CheckCurrentState();
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;

            case State.Death:

                break;

            default:
            case State.Idle:

                break;
        }
    }



    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChaisingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.Chasing;
            }
        }

        if (isAttackingEnemy)
        {
            if(distanceToPlayer <= attackingDistance)
            {
                if (Player.Instance.IsAlive())
                {
                    newState = State.Attacking;
                }
                else
                {
                    newState = State.Roaming;
                }
            }
            
        }

        if (newState != state)
        {
            if (newState == State.Chasing)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.speed = chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                roamingTime = 0f;
                navMeshAgent.speed = roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                navMeshAgent.ResetPath();
            }
            state = newState;
        }
    }

    private void ChasingTarget()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void AttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            nextAttackTime = Time.time + attackRate;
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDir(lastPos, transform.position);
            }
            else if (state == State.Attacking)
            {
                ChangeFacingDir(transform.position, Player.Instance.transform.position);
            }

            lastPos = transform.position;
            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
    }

    private void Roaming()
    {
        startingPos = transform.position;
        roamPos = GetRoamingPosition();
        navMeshAgent.SetDestination(roamPos);
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPos + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDir(Vector3 sourcePos, Vector3 targetPos)
    {
        if (sourcePos.x > targetPos.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

        }

    }


}
