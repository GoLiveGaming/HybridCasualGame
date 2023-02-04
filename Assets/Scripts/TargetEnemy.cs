
using UnityEngine;
using UnityEngine.AI;

public class TargetEnemy : MonoBehaviour
{
    public GameObject gameObjectSelf { get { return this.gameObject; } }
    public Transform goal;
    public Stats stats;
    public Canvas canvas;

    private void Awake()
    {
        if (!stats) GetComponent<Stats>();
    }
    void Start()
    {
        goal = GameObject.Find("Player Tower").GetComponent<Transform>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    private void FixedUpdate()
    {
        canvas.transform.LookAt(Camera.main.transform);
        if (stats.Damage > 0)
        {
            stats.Health -= 5 * Time.deltaTime;
            stats.Damage -= Time.deltaTime;

            Debug.Log(stats.Damage);
        }
    }

    
}
