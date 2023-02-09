using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(Stats))]
public class NPCManagerScript : MonoBehaviour
{
    [Header("Current State(ReadOnly)"), Space(2)]
    public NPCStates activeState;

    [Header("NPC PARAMETERS"), Space(2)]
    [Header("AutoCache Componenets")]
    private MainPlayerControl _playerControl;

    [Header("Parameters")]
    [SerializeField] private float defaultMoveSpeed = 1f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float attackDamage = 1f;


    [Header("AI BEHAVIOR PARAMETERS"), Space(2)]

    [SerializeField, Range(0.1f, 5f)] private float stateRefreshDelay = 1f;
    [SerializeField] private LayerMask playerTowerLayer;
    [SerializeField] private float timeSinceLastStateRefresh = 0f;

    [HideInInspector] public NavMeshAgent _agent;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public GameObject _player;
    [HideInInspector] public Stats _stats;

    public GameObject gameObjectSelf { get { return this.gameObject; } }

    //States initialization
    private NPCBaseState _currentState;
    public readonly NPCPursueState PursueState = new NPCPursueState();
    public readonly NPCAttackState AttackState = new NPCAttackState();

    public enum NPCStates
    {
        Pursue,
        Attack
    }
    void Start()
    {
        _playerControl = MainPlayerControl.Instance;
        _player = _playerControl.gameObject;
        _stats = GetComponent<Stats>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = defaultMoveSpeed;
        _agent.stoppingDistance = stoppingDistance;

        if (!isPlayerAvailable())
            Destroy(this.gameObject);


        //Adding random parity in state refresh delay to reduce stress on cpu when large quantity of npc's are used
        stateRefreshDelay = Mathf.Clamp(Random.Range(stateRefreshDelay - 1f, stateRefreshDelay + 1f), 0, 5f);

        _currentState = PursueState;
        _currentState.EnterState(this);
    }
    private void Update()
    {

        if (CanRefreshState()) _currentState.UpdateState(this);
    }
    public void SwitchState(NPCBaseState state)
    {
        _currentState = state;
        _currentState.EnterState(this);
    }

    public void UpdateDestination()
    {
        if (isPlayerAvailable())
            SetTargetTower();
        else
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }
    }
    public void SetTargetTower()
    {
        if (_playerControl.activePlayerTowersList.Count == 0
            || _playerControl.activePlayerTowersList == null) return;

        int rndnum = Random.Range(0, 100);

        if (rndnum > 20)
        {
            float closestDistance = Mathf.Infinity;
            GameObject targetTower = null;
            foreach (PlayerTower tower in _playerControl.activePlayerTowersList)
            {
                float distance = Vector3.Distance(transform.position, tower.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetTower = tower.gameObject;
                }
            }
            _agent.SetDestination(targetTower.transform.position);
        }
        else
        {
            int rndTower = Random.Range(0, _playerControl.activePlayerTowersList.Count);
            _agent.SetDestination(_playerControl.activePlayerTowersList[rndTower].transform.position);
        }
    }
    public bool InTargetProximity()
    {
        if (CheckIfTargetInFront()) return true;

        return false;
    }

    private bool CheckIfTargetInFront()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, stoppingDistance, playerTowerLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        return false;
    }
    private bool CanRefreshState()
    {
        if (isPlayerAvailable())
        {
            timeSinceLastStateRefresh += Time.deltaTime;
            if (stateRefreshDelay < timeSinceLastStateRefresh)
            {
                timeSinceLastStateRefresh = 0;
                return true;
            }
            return false;
        }
        else return false;
    }
    private bool isPlayerAvailable()
    {
        if (_playerControl.activePlayerTowersList.Count != 0) return true;
        else return false;

    }



    //ANIMATION EVENTS

    /// <summary>
    /// Animation Event For attacking the player
    /// </summary>
    public void AttackPlayer()
    {
        if (isPlayerAvailable())
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, stoppingDistance, playerTowerLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
            }
        }
    }
}
