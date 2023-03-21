using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodBulletAOE : BulletAOE
{
    private float slowedDownSpeed;
    private float slowDownDuration;
    private Color associatedColor;
    public void Initialize(float slowedSpeed, float slowDuration, Color _associatedColor)
    {
        slowedDownSpeed = slowedSpeed;
        slowDownDuration = slowDuration;
        associatedColor = _associatedColor;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NPCManagerScript hitNPC))
        {
            hitNPC._stats.damageNumberColor = associatedColor;
            hitNPC._stats.SlowDownMoveSpeed(slowedDownSpeed, slowDownDuration);
        }

    }
}
