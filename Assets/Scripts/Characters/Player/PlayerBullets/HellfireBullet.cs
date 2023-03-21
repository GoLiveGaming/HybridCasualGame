using UnityEngine;

public class HellfireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]

    public float damageDuration = 5f;
    [SerializeField] private HellfireBulletAOE bulletAOE;

    private void Awake()
    {
        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.FireBallProjectile);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        {
            if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.FireBallHit);
            if (other.TryGetComponent(out NPCManagerScript npcManager))
                StartAttack(npcManager);
            if (other.TryGetComponent(out BulletAOE aoe))
            {
                aoe.StartExpandingAOE();
                DestroySelf();
            }
        }

    }

    protected override void StartAttack(NPCManagerScript hitNPC)
    {
        //END ATTACK
        bulletAOE.StartAOEEffect();
        bulletAOE.transform.SetParent(null);
        bulletAOE.Initialize(damageDuration, damage, associatedColor);

        DestroySelf();

    }
}
