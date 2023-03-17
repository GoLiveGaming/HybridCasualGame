using DG.Tweening;
using UnityEngine;
public class FireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float damageDuration = 5f;

    [SerializeField] private ParticleSystem ExplosionPrefab;

    private void Awake()
    {
        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.FireBallProjectile);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        {
            other.TryGetComponent(out NPCManagerScript npcManager);
            if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.FireBallHit);
            StartAttack(npcManager);
        }
    }

    protected override void StartAttack(NPCManagerScript hitNPC)
    {
        if (!hitNPC) return;
        if (hitNPC._stats)
        {
            hitNPC._stats.damageNumberColor = associatedColor;
            hitNPC._stats.AddDamageOverTime(damageDuration, damage);
        }

        //END ATTACK
        ParticleSystem Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Explosion.transform.DOScale(Vector3.one, Explosion.main.duration).OnComplete(() =>
        {
            Destroy(Explosion.gameObject);
        });
        Destroy(gameObject);
    }
}
