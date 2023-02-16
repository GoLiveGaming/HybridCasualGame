using DG.Tweening;
using UnityEngine;

public class WaterBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
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

        hitNPC._stats.SlowDownMoveSpeed(slowedDownSpeed, slowDownDuration);
        hitNPC._stats.AddDamage(damage);

        //END ATTACK
        Destroy(gameObject);
    }
}
