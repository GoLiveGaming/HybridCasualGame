using UnityEngine;

public class FloodBullet : Bullet
{
    [Space(2), Header("BULLET EXTENDED PROPERTIES")]
    [SerializeField] private BulletAOE bulletAOE;
    public float slowedDownSpeed = 0.25f;
    public float slowDownDuration = 3;

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
        bulletAOE.StartAOEEffect(this);
        bulletAOE.transform.SetParent(null);
        //END ATTACK
        Destroy(gameObject);
    }
}
