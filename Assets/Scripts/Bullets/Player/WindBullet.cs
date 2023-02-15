using DG.Tweening;
using UnityEngine;

public class WindBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private GameObject aoeVisualObj;
    [SerializeField] private float aoeLifetime = 0.15f;
    [SerializeField] private float impactAreaRadius = 5f;
    [SerializeField] private float impactForce = 10f;
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

        GameObject spawnedAOE = Instantiate(aoeVisualObj, this.transform.position, Quaternion.identity);

        if (spawnedAOE) spawnedAOE.transform.DOScale(Vector3.one * impactAreaRadius, aoeLifetime);
        if (spawnedAOE) Destroy(spawnedAOE, aoeLifetime);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                //Add Explosion Force
                Vector3 direction = rb.transform.position - transform.position;
                rb.AddForce(direction.normalized * impactForce, ForceMode.Impulse);

                //Modify Stats
                rb.TryGetComponent(out NPCManagerScript npc);
                if (npc) npc._stats.AddDamage(damage);
            }
        }
        //END ATTACK
        Destroy(gameObject);
    }

}
