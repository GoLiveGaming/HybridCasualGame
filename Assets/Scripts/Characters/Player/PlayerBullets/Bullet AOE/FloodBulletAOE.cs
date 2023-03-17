using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodBulletAOE : BulletAOE
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (m_OwnerBullet.gameObject.TryGetComponent(out FloodBullet floodBullet))
        {

            if (other.TryGetComponent(out NPCManagerScript hitNPC))
            {
                hitNPC._stats.damageNumberColor = floodBullet.associatedColor;
                hitNPC._stats.SlowDownMoveSpeed(floodBullet.slowedDownSpeed, floodBullet.slowDownDuration);
            }
        }

    }
}
