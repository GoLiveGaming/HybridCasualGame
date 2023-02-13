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
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, stoppingDistance, playerTowerLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                hit.transform.TryGetComponent(out Stats stats);
                if (stats) stats.AddDamage(attackDamage);
            }
        }
    }




}
