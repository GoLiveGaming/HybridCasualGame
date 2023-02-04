using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent), (typeof(Animator)))]
public class NPCStateManager : MonoBehaviour
{
    [Header("AI Parameters"), Space(2)]

    [SerializeField] private float defaultMoveSpeed = 1;
    [SerializeField] private float stoppingDistance = 2;
    [SerializeField] private LayerMask playerTowerLayer;
    [SerializeField, Range(0f, 5f)] private float stateRefreshDelay = 1f;
    public float timeSinceLastStateRefresh = 0f;



    [Header("Animation Parameters"), Space(2)]
    public bool doAnims;
    public string[] pursueAnimBoolNames = new string[] { "Idle_01", "Idle_02", "Idle_03" };
    public string[] attackAnimBoolNames = new string[] { "Roam_01", "Roam_02", "Roam_03" };
    public string[] evadeAnimBoolNames = new string[] { "Evade_01", "Evade_02", "Evade_03" };

    [Header("Current State Status"), Space(2)]
    public NPCStates activeState;


    //Local Variables initialization
    private Vector3 startPosition;
    private float timer;


    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GameObject player;


    //States initialization
    public NPCBaseState currentState;
    public NPCPursueState pursueState = new NPCPursueState();
    public NPCAttackState attackState = new NPCAttackState();

    public enum NPCStates
    {
        Pursue,
        Attack
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = MainPlayerControl.instance.gameObject;

        agent.speed = defaultMoveSpeed;
        agent.stoppingDistance = stoppingDistance;
        startPosition = this.transform.position;


        currentState = pursueState;
        currentState.EnterState(this, animator, player);
    }


    public void Update()
    {
        if (CanRefreshState()) currentState.UpdateState(this, animator, player);
    }
    public void updateNPC()
    {
        if (CanRefreshState()) currentState.UpdateState(this, animator, player);
    }
    public void SwitchState(NPCBaseState state)
    {
        currentState = state;
        currentState.EnterState(this, animator, player);
    }

    private bool CanRefreshState()
    {
        if (stateRefreshDelay == 0) return true;

        timeSinceLastStateRefresh += Time.deltaTime;
        if (stateRefreshDelay < timeSinceLastStateRefresh)
        {
            timeSinceLastStateRefresh = 0;
            return true;
        }
        else return false;
    }
    public void ResetAnimatorBools()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                animator.SetBool(parameter.name, false);
        }
    }
    public Vector3 GetTargetLocation()
    {
        return player.transform.position;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    public bool HasRechedDestination()
    {
        if (CheckIfHasReachedTarget()) return true;

        return false;
    }

    private bool CheckIfHasReachedTarget()
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


}
