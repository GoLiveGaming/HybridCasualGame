using DG.Tweening;
using UnityEngine;

public class IceBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float slowedDownSpeed = 0;
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

        //END ATTACK
        Destroy(gameObject);
    }
}
