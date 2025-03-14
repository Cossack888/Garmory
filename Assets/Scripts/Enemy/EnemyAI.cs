using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static FightControlls;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Defend
    }

    public EnemyState currentState;
    private Transform player;
    [SerializeField] float baseMovementSpeed = 5;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float patrolRadius = 10f;
    private float lastAttackTime = 0f;
    private AttackMode attackMode = AttackMode.Single;
    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    public Action<AnimationType> OnAttack;
    public Action<bool> OnDefend;
    public Action OnDeath;
    private Health health;
    private Health targetHealth;
    private FightControlls controlls;
    private PlayerStatistics playerStatistics;
    private Coroutine defending;
    private Animator animator;
    private AnimationManager animationManager;
    private Coroutine attackModeCoroutine;
    private Statistics statistics;
    private float playerCritChance;
    private bool isDead;
    private bool isPaused;
    void OnEnable()
    {
        statistics = GetComponent<Statistics>();
        playerStatistics = player.GetComponent<PlayerStatistics>();
        playerCritChance = playerStatistics.CriticalStrikeChance;
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Patrol;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.speed = baseMovementSpeed * (statistics.MovementSpeed / 100);
        animator = GetComponentInChildren<Animator>();
        SetNewPatrolPoint();
        SwitchMode(0);
    }
    public void ToggleRun(bool state)
    {
        float speedBonus = 1 + statistics.MovementSpeed / 100f;
        agent.speed = baseMovementSpeed * speedBonus * (state ? 2 : 1);
    }
    public bool IsDead()
    {
        return isDead;
    }
    private void Awake()
    {
        controlls = FindAnyObjectByType<FightControlls>();
        if (controlls != null)
        {
            controlls.OnAttack += DefendFromPlayer;
        }
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += HandleDeath;
            health.OnHit += PushBack;
        }
        player = FindAnyObjectByType<PlayerMovement>().transform;
        targetHealth = player.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.OnDeath += HandlePlayerDeath;
        }

        animationManager = GetComponent<AnimationManager>();
    }
    public Statistics GetStatistics()
    {
        return statistics;
    }
    private bool IsInDetectionRange()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }
    private bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }
    private void OnDisable()
    {
        if (controlls != null)
        {
            controlls.OnAttack -= DefendFromPlayer;
        }
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
            health.OnHit -= PushBack;
        }
        if (targetHealth != null)
        {
            targetHealth.OnDeath -= HandlePlayerDeath;
        }
    }
    void Update()
    {
        if (isPaused) { return; }
        animationManager.AnimateMovement(agent.velocity.x, agent.velocity.z);
        if (player != null && IsInDetectionRange())
        {
            agent.transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        switch (currentState)
        {
            case EnemyState.Patrol:
                ToggleRun(false);
                Patrol();
                break;
            case EnemyState.Chase:
                ToggleRun(true);
                ChasePlayer();
                break;
            case EnemyState.Attack:
                ToggleRun(false);
                AttackPlayer();
                break;
            case EnemyState.Defend:
                DefendFromPlayer(AnimationType.SimpleAttack);
                break;

        }
    }
    private void PushBack(SingleAnimations hit)
    {
        if (player != null)
        {
            Vector3 pushDirection = (transform.position - player.transform.position).normalized;
            agent.Move(pushDirection * 2);
        }
    }
    public void HandleDeath()
    {
        OnDeath?.Invoke();
        isDead = true;
        this.enabled = false;
    }
    private void HandlePlayerDeath()
    {
        player = null;
        targetHealth.OnDeath -= HandlePlayerDeath;
        currentState = EnemyState.Patrol;
        SetNewPatrolPoint();
    }
    private IEnumerator SwitchAttackModeRoutine()
    {
        while (true)
        {
            if (player == null)
                break;
            yield return new WaitForSeconds(5f);
            int randomMode = UnityEngine.Random.Range(0, 3);
            SwitchMode(randomMode);
        }
    }
    public void SwitchMode(int number)
    {

        if (attackModeCoroutine != null)
        {
            StopCoroutine(attackModeCoroutine);
            attackModeCoroutine = null;
        }


        switch (number)
        {
            case 0:
                attackMode = AttackMode.Single;
                break;
            case 1:
                attackMode = AttackMode.Combo;
                break;
            case 2:
                attackMode = AttackMode.Reckless;
                break;
            default:

                break;
        }
        if (attackModeCoroutine == null)
        {
            attackModeCoroutine = StartCoroutine(SwitchAttackModeRoutine());
        }

    }

    void Patrol()
    {
        if (player != null)
        {
            if (IsInDetectionRange())
            {
                currentState = EnemyState.Chase;
            }
        }
        agent.isStopped = false;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewPatrolPoint();
        }

        agent.SetDestination(patrolTarget);
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            agent.isStopped = false;


            Vector3 directionToPlayer = (player.position - transform.position).normalized;


            float flankOffset = Random.Range(-3f, 3f);
            Vector3 rightVector = Vector3.Cross(Vector3.up, directionToPlayer);
            Vector3 flankDirection = player.position + rightVector * flankOffset;


            float strafeSpeed = 2f;
            float strafeAmount = Mathf.Sin(Time.time * strafeSpeed) * 1.5f;


            Vector3 finalPosition = flankDirection + rightVector * strafeAmount;


            agent.SetDestination(finalPosition);


            if (IsInAttackRange())
            {
                currentState = EnemyState.Attack;
            }


            if (!IsInDetectionRange())
            {
                currentState = EnemyState.Patrol;
                SetNewPatrolPoint();
            }
        }
    }
    void AttackPlayer()
    {
        if (player != null)
        {
            agent.isStopped = true;


            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {

                    switch (attackMode)
                    {
                        case AttackMode.Single:
                            OnAttack?.Invoke(AnimationType.SimpleAttack); break;
                        case AttackMode.Combo:
                            OnAttack?.Invoke(AnimationType.ComboAttack); break;
                        case AttackMode.Reckless:
                            OnAttack?.Invoke(AnimationType.JumpAttack); break;
                    }
                    lastAttackTime = Time.time;
                }
            }
            else
            {

                currentState = EnemyState.Chase;
            }
        }
    }
    public void TogglePause(bool state)
    {
        isPaused = state;
        if (state)
        {
            animator.speed = 0f;
            StopAllCoroutines();
            agent.isStopped = true;
        }
        else
        {
            animator.speed = 1f;
            SwitchMode(0);
            agent.isStopped = false;
        }
    }
    public void DefendFromPlayer(AnimationType type)
    {
        if (player == null) return;

        float critChance = playerCritChance / 100;
        float counterChance = NormalisingUtils.NormalizeStat(statistics.Defense);

        bool hasDefended = Random.value < critChance;
        bool counterAttack = Random.value < counterChance;

        if (counterAttack)
        {

            OnAttack?.Invoke(AnimationType.SimpleAttack);
            return;
        }

        if (hasDefended)
        {

            OnDefend?.Invoke(true);

            if (defending == null)
            {
                defending = StartCoroutine(StopDefending());
            }
        }
    }
    public IEnumerator StopDefending()
    {
        yield return new WaitForSeconds(2f);
        OnDefend?.Invoke(false);
        defending = null;
    }
    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
        else
        {
            SetNewPatrolPoint();
        }
    }
}
