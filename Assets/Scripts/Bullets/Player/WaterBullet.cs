using UnityEngine;

public class WaterBullet : Bullet
{
    [SerializeField] private float slowedDownSpeed = 0.25f;
    [SerializeField] private float slowDownDuration = 3;
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
        if (hitNPC._stats) hitNPC._stats.SlowDownMoveSpeed(slowedDownSpeed, slowDownDuration);

        //END ATTACK
        Destroy(gameObject);
    }
}
