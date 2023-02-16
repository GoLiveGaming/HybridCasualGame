using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : NPCManagerScript
{

    public GameObject attackBulletPrefab;
    /// <summary>
    /// Animation Event For attacking the player
    /// </summary>
    public override void AttackPlayer()
    {
        if (isPlayerAvailable())
        {
            if (targetTower != null)
            {
                GameObject bullet = Instantiate(attackBulletPrefab, transform.position, transform.rotation);
                bullet.GetComponent<Bullet>().InitializeBullet(targetTower.transform.position);
            }
        }
    }

}
