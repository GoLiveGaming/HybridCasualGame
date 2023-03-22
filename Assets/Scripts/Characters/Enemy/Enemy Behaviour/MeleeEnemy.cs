using UnityEngine;

public class MeleeEnemy : NPCManagerScript
{
    /// <summary>
    /// Animation Event For attacking the player
    /// </summary>
    public override void AttackPlayer()
    {
        if (IsPlayerAvailable())
        {
            // Does the ray intersect any objects excluding the player layer
            Vector3 spawnPos = firePointTransform != null ? firePointTransform.position : transform.position;

            if (Physics.Raycast(spawnPos, transform.TransformDirection(Vector3.forward), out RaycastHit hit, detectionRange, playerTowerLayer))
            {
              //  Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
                if(AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.EnemyHit);
                ParticleSystem deathParticle = Instantiate(_playerControl.EnemyParticles[0], transform.position + new Vector3(1.2f, 3f, -2.5f), Quaternion.identity);
                Destroy(deathParticle.gameObject, deathParticle.main.duration);
            }
        }
    }
}
