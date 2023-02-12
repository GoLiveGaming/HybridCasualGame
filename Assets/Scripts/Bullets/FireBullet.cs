using UnityEngine;
public class FireBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {

        if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        {
            other.TryGetComponent(out NPCManagerScript npcManager);
            StartAttack(npcManager);
        }
    }

    protected override void StartAttack(NPCManagerScript hitNPC)
    {
        if (!hitNPC) return;
        if (hitNPC._stats) hitNPC._stats.AddDamageOverTime(5, damage);

        //END ATTACK
        Destroy(gameObject);
    }
}
