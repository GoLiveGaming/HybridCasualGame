using UnityEngine;

public class MeleeEnemy : NPCManagerScript
{
    /// <summary>
    /// Animation Event For attacking the player
    /// </summary>
    public override void AttackPlayer()
    {
        if (isPlayerAvailable())
        {
            // Does the ray intersect any objects excluding the player layer
            Vector3 spawnPos = firePointTransform != null ? firePointTransform.position : transform.position;

            if (Physics.Raycast(spawnPos, transform.TransformDirection(Vector3.forward), out RaycastHit hit, detectionRange, playerTowerLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
            }
        }
    }
}
