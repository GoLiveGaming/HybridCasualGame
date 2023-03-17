using UnityEngine;

public class HellfireBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]

    [SerializeField] private BulletAOE bulletAOE;

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
        bulletAOE.StartAOEEffect(this);
        bulletAOE.transform.SetParent(null);

        //END ATTACK
        Destroy(gameObject);
    }
}
