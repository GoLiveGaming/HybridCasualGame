using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : NPCManagerScript
{

    [SerializeField] private GameObject attackBulletPrefab;
    /// <summary>
    /// Animation Event For attacking the player
    /// </summary>
    public override void AttackPlayer()
    {
        if (isPlayerAvailable())
        {
            if (targetTower != null)
            {
                Vector3 spawnPos = firePointTransform ? firePointTransform.position : transform.position;

                GameObject bullet = Instantiate(attackBulletPrefab, spawnPos, transform.rotation);
                bullet.GetComponent<Bullet>().InitializeBullet(targetTower.transform.position);
            }
        }
    }

}
