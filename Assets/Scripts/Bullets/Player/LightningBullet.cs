using DG.Tweening;
using UnityEngine;

public class LightningBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private GameObject aoeVisualObj;
    [SerializeField] private float aoeLifetime = 0.15f;
    [SerializeField] private Vector3 aoeSpawnOffset = Vector3.zero;

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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        GameObject spawnedAOE = Instantiate(aoeVisualObj, this.transform.position + aoeSpawnOffset, Quaternion.identity);

        if (spawnedAOE) spawnedAOE.transform.DOMove(this.transform.position, aoeLifetime);
        if (spawnedAOE) Destroy(spawnedAOE, aoeLifetime);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                //Add Explosion Force
                rb.AddExplosionForce(explosionForce, transform.position + new Vector3(0, 0, -1), explosionRadius, 1f, ForceMode.Impulse);

                //Modify Stats

                rb.TryGetComponent(out NPCManagerScript npc);
                if (npc) npc._stats.AddDamageOverTime(5, damage);
            }
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