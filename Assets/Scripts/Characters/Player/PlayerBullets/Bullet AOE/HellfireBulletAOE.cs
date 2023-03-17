using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellfireBulletAOE : BulletAOE
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NPCManagerScript hitNPC))
        {
            hitNPC._stats.damageNumberColor = m_OwnerBullet.associatedColor;
            hitNPC._stats.AddDamageOverTime(m_OwnerBullet.damageDuration, m_OwnerBullet.damage);
        }
    }
}
