using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            other.TryGetComponent(out Stats stats);
            if (stats) stats.AddDamage(damage);
            Destroy(gameObject);
            //StartAttackTower(tower);
        }
        //if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        //{
        //    other.TryGetComponent(out PlayerTower tower);
        //    StartAttackTower(tower);
        //}
    }

    private void OnDestroy()
    {
        if (MainPlayerControl.Instance)
        {
            if(AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.EnemyHit);
            ParticleSystem deathParticle = Instantiate(MainPlayerControl.Instance.EnemyParticles[0], transform.position, Quaternion.identity);
            Destroy(deathParticle.gameObject, deathParticle.main.duration);
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
