using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyBullet : Bullet
{
    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.TryGetComponent(out PlayerTower mainPayer);
            StartAttackTower(mainPayer);
        }
        //if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        //{
        //    other.TryGetComponent(out MainPlayerControl mainPayer);
        //    StartAttackTower(mainPayer);
        //}
    }

    protected override void StartAttackTower(PlayerTower hitMain)
    {
        if (!hitMain) return;
        if (hitMain._stats) hitMain._stats.AddDamage(damage);

        //END ATTACK
        Destroy(gameObject);
    }
}
