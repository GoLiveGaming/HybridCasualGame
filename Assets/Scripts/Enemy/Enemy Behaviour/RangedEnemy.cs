using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : NPCManagerScript
{
    // [SerializeField] private float rangedEnemyStoppedDistance = 25f;
    // [SerializeField] private LayerMask mainTowerLayer;

    public GameObject attackBulletPrefab;

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
            GameObject bullet = Instantiate(attackBulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().initializeBullet(_playerControl.transform);
        }
    }




}
