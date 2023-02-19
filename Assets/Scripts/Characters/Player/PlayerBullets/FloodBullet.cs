using DG.Tweening;
using UnityEngine;

public class FloodBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private float slowedDownSpeed = 0.25f;
    [SerializeField] private float slowDownDuration = 3;
    [SerializeField] private GameObject aoeVisualObj;
    [SerializeField] private float aoeLifetime = 0.15f;
    [SerializeField] private float aoeRadius = 5f;

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

        Collider[] hitColliders = new Collider[10];

        int numTargets = Physics.OverlapSphereNonAlloc(transform.position, aoeRadius, hitColliders, collisionLayerMask);

        GameObject spawnedAOE = Instantiate(aoeVisualObj, this.transform.position, Quaternion.identity);

        if (spawnedAOE) spawnedAOE.transform.DOScale(Vector3.one * aoeRadius, aoeLifetime);
        if (spawnedAOE) Destroy(spawnedAOE, aoeLifetime);

        if (numTargets > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider)
                {
                    hitCollider.TryGetComponent(out NPCManagerScript npc);
                    npc._stats.SlowDownMoveSpeed(slowedDownSpeed, slowDownDuration);
                }
            }
        }
        //END ATTACK
        Destroy(gameObject);
    }
}
