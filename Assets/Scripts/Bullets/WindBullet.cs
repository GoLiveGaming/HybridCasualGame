using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBullet : Bullet
{
    [SerializeField] private GameObject aoeVisualObj;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
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

        if (spawnedAOE) spawnedAOE.transform.DOScale(Vector3.one * explosionRadius, 0.25f);
        if (spawnedAOE) Destroy(spawnedAOE, 2f);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                //Add Explosion Force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);

                //Modify Stats
                rb.GetComponent<NPCManagerScript>()._stats.AddDamage(5f);
            }
        }


        //END ATTACK
        Destroy(gameObject);
    }

}
