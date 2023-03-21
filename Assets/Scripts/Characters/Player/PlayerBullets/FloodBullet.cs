using UnityEngine;

public class FloodBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]

    [SerializeField] private FloodBulletAOE bulletAOE;
    public float slowedDownSpeed = 0.25f;
    public float slowDownDuration = 3;

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

        bulletAOE.Initialize(slowedDownSpeed, slowDownDuration, associatedColor);

        DestroySelf();
    }
}
