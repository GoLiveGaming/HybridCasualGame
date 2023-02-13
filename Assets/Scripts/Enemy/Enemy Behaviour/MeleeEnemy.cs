using UnityEngine;

public class MeleeEnemy : NPCManagerScript
{
    // [SerializeField] private float rangedEnemyStoppedDistance = 25f;
    


    private void Update()
    {
        if (CanRefreshState())
        {
            _currentState.UpdateState(this);

            if (InTargetProximity())
            {
                // _agent.ResetPath();
                _agent.isStopped = true;
                //  ExitState(npcManager);
            }
            else
            {
                _agent.isStopped = false;
            }
        }

    }

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
              //  Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
            }
        }
    }




}
