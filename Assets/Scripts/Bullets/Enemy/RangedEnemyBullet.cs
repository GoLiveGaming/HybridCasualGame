using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        {
            other.TryGetComponent(out PlayerTower tower);
            StartAttackTower(tower);
        }
    }

    protected override void StartAttackTower(PlayerTower hitMain)
    {
        if (!hitMain) return;
        if (hitMain._stats) hitMain._stats.AddDamage(damage);

        //END ATTACK
        Destroy(gameObject);
    }
}
