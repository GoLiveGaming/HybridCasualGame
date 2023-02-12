using UnityEngine;
public class FireBullet : Bullet
{
    protected void OnTriggerEnter(Collider other)
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
        if (hitNPC._stats) hitNPC._stats.AddDamageOverTime(5, damage);

        gameObject.name = "changed";
        //END ATTACK
        Destroy(gameObject);
    }

    //private void FixedUpdate()
    //{
    //    RaycastHit hit;
    //    // Does the ray intersect any objects excluding the player layer
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f, collisionLayerMask))
    //    {
    //        hit.transform.TryGetComponent(out NPCManagerScript npcManager);
    //        StartAttack(npcManager);
    //    }
    //}
}
