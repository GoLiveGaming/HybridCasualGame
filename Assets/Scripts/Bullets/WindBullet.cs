using DG.Tweening;
using UnityEngine;

public class WindBullet : Bullet
{
    [SerializeField] private GameObject aoeVisualObj;
    [SerializeField] private float aoeLifetime = 0.15f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
    protected void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayerMask))
        {
            other.TryGetComponent(out NPCManagerScript npcManager);
            StartAttack(npcManager);
        }
        else
        {

        }
    }

    protected override void StartAttack(NPCManagerScript hitNPC)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        GameObject spawnedAOE = Instantiate(aoeVisualObj, this.transform.position, Quaternion.identity);

        if (spawnedAOE) spawnedAOE.transform.DOScale(Vector3.one * explosionRadius, aoeLifetime);
        if (spawnedAOE) Destroy(spawnedAOE, aoeLifetime);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                //Add Explosion Force
                rb.AddExplosionForce(explosionForce, transform.position + new Vector3(0, 0, -1), explosionRadius, 1f, ForceMode.Impulse);

                //Modify Stats
                rb.GetComponent<NPCManagerScript>()._stats.AddDamage(damage);
            }
        }


        //END ATTACK
        Destroy(gameObject);
    }

}
