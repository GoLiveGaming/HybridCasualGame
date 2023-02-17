using DG.Tweening;
using UnityEngine;
public class FireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float damageDuration = 5f;

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
        if (hitNPC._stats) hitNPC._stats.AddDamageOverTime(damageDuration, damage);

        //END ATTACK
        ParticleSystem Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Explosion.transform.DOScale(Vector3.one, Explosion.main.duration).OnComplete(() =>
        {
            Destroy(Explosion.gameObject);
        });
        Destroy(gameObject);
    }
}
