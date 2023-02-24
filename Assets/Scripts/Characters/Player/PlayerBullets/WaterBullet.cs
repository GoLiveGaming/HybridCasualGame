using DG.Tweening;
using UnityEngine;

public class WaterBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float slowedDownSpeed = 0.25f;
    [SerializeField] private float slowDownDuration = 3;
    [SerializeField] private ParticleSystem ExplosionPrefab;
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
        if (hitNPC._stats)
        {
            hitNPC._stats.damageNumberColor = associatedColor;
            hitNPC._stats.AddDamage(damage);
        }
        ParticleSystem Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Explosion.transform.DOScale(Vector3.one, Explosion.main.duration).OnComplete(() =>
        {
            Destroy(Explosion.gameObject);
        });
        //END ATTACK
        Destroy(gameObject);
    }
}
