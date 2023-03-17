using UnityEngine;

public class HellfireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float damageDuration = 5f;

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
        Destroy(gameObject);
    }
}
