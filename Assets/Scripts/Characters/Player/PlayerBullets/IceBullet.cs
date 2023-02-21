using DG.Tweening;
using UnityEngine;

public class IceBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float slowedDownSpeed = 0;
    [SerializeField] private float slowDownDuration = 3;

    [SerializeField] private ParticleSystem ExplosionPrefab;

    private void Awake()
    {
        if(AudioManager.Instance)AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.IceAttack);
    }

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

        ParticleSystem Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Explosion.transform.DOScale(Explosion.transform.localScale, Explosion.main.duration).OnComplete(() =>
        {
            Destroy(Explosion.gameObject);
        });
        //END ATTACK
        Destroy(gameObject);
    }
}
