using UnityEngine.AI;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Stats))]


public class NPCManagerScript : MonoBehaviour
{
    [Header("Current State(ReadOnly)"), Space(2)]
    public NPCStates activeState;
    [SerializeField] internal EnemyTypes enemyType;

    [Header("NPC PARAMETERS"), Space(2)]
    [Header("AutoCache Componenets")]
    internal MainPlayerControl _playerControl;

    [Header("Parameters")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float animatorSpeed = 1f;
    [SerializeField] internal float attackDamage = 1f;
    [SerializeField] internal float detectionRange = 2f;
    [SerializeField] internal Transform firePointTransform;


    [Header("AI BEHAVIOR PARAMETERS"), Space(2)]

    [SerializeField, Range(0.1f, 5f)] private float stateRefreshDelay = 1f;
    [SerializeField, ReadOnly] private float timeSinceLastStateRefresh = 0f;
    [SerializeField] internal LayerMask playerTowerLayer;

    public NavMeshAgent _agent;
    [HideInInspector] public Stats _stats;
    [HideInInspector] public Animator _animator;



    Rigidbody _rigidbody;
    float m_TurnAmount;
    float m_ForwardAmount;
    float defaultMoveSpeed;
    bool m_canUpdate;
    Vector3 m_GroundNormal;



    public GameObject GameObjectSelf { get { return this.gameObject; } }

    public void SetMoveSpeed(float value)
    {
        if (value == 0) _agent.isStopped = true; _agent.ResetPath();
        moveSpeed = value; 
        _agent.speed = moveSpeed;
    }
    public void ResetMoveSpeed() { _agent.ResetPath(); moveSpeed = defaultMoveSpeed; _agent.speed = moveSpeed; }

    //States initialization
    internal NPCBaseState _currentState;
    public readonly NPCPursueState PursueState = new();
    public readonly NPCAttackState AttackState = new();

    [ReadOnly] public GameObject targetTower = null;

    bool isInitialized;
    public enum NPCStates
    {
        Pursue,
        Attack
    }
    public void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (CanRefreshState())
        {
            _currentState.UpdateState(this);
        }
        if (m_canUpdate && _agent.remainingDistance > 0)
        {
            UpdateAnims(_agent.desiredVelocity);
        }
    }
    internal void Initialize()
    {
        if (isInitialized)
            return;
        isInitialized = true;
        _playerControl = MainPlayerControl.Instance;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _stats = GetComponent<Stats>();
        _agent = GetComponent<NavMeshAgent>();

        _stats.InitializeStats();

        m_canUpdate = true;
        defaultMoveSpeed = moveSpeed;
        _agent.speed = defaultMoveSpeed;
        _animator.speed = animatorSpeed;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //Adding random parity in state refresh delay to reduce stress on cpu when large quantity of npc's are spawning
        stateRefreshDelay = Mathf.Clamp(Random.Range(stateRefreshDelay - 1f, stateRefreshDelay + 1f), 0f, 5f);

        _currentState = PursueState;
        _currentState.EnterState(this);
    }
    public void UpdateAnims(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        _animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
    }

    public void SwitchState(NPCBaseState state)
    {
        _currentState = state;
        _currentState.EnterState(this);
    }


    public void UpdateDestination()
    {
        if (IsPlayerAvailable())
            SetTargetTower();
    }
    public void SetTargetTower()
    {

        if (_playerControl.activePlayerTowersList.Count == 0
            || _playerControl.activePlayerTowersList == null) { Debug.LogError("NO TARGET DESTINATION FOUND FOR" + this); return; }

        float closestDistance = Mathf.Infinity;
        GameObject closestTower = null;

        foreach (PlayerUnitBase tower in _playerControl.activePlayerTowersList)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = tower.gameObject;
            }
        }
        if (closestTower)
        {
            if (!targetTower || !_agent.hasPath)
            {
                targetTower = closestTower;
                _agent.SetDestination(targetTower.transform.position);
            }
            else if (closestTower != targetTower)
            {
                targetTower = closestTower;
                _agent.SetDestination(targetTower.transform.position);
            }
        }
    }

    public bool InTargetProximity()
    {
        return CheckIfTargetInFront();
    }

    private bool CheckIfTargetInFront()
    {
        Debug.DrawRay(transform.position, firePointTransform.TransformDirection(Vector3.forward) * detectionRange, Color.green);
        if (Physics.Raycast(transform.position, firePointTransform.TransformDirection(Vector3.forward), out RaycastHit hit, detectionRange, playerTowerLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            return true;
        }

        return false;
    }

    internal bool CanRefreshState()
    {
        if (IsPlayerAvailable())
        {
            timeSinceLastStateRefresh += Time.deltaTime;
            if (stateRefreshDelay < timeSinceLastStateRefresh)
            {
                timeSinceLastStateRefresh = 0;
                return true;
            }
        }
        return false;
    }
    internal bool IsPlayerAvailable()
    {
        return _playerControl._mainPlayerTower;
    }

    //ANIMATION EVENTS
    public virtual void AttackPlayer()
    {
        if (IsPlayerAvailable())
        {
            Vector3 spawnPos = firePointTransform ? firePointTransform.position : transform.position;

            if (Physics.Raycast(spawnPos, transform.TransformDirection(Vector3.forward), out RaycastHit hit, detectionRange, playerTowerLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
            }
        }
    }

    private void OnDestroy()
    {
        if (_playerControl)
        {
            ParticleSystem deathParticle = Instantiate(_playerControl.EnemyParticles[1], transform.position + new Vector3(0, 5), Quaternion.identity);
            Destroy(deathParticle.gameObject, deathParticle.main.duration);
        }

    }
}
