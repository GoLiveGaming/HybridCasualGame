using UnityEngine;
public class FireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float damageDuration = 5f;
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
        if (hitNPC._stats) hitNPC._stats.AddDamageOverTime(damageDuration, damage);

        //END ATTACK
        Destroy(gameObject);
    }
}
