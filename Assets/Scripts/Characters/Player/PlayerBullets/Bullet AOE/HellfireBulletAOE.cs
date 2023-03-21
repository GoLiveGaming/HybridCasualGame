using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellfireBulletAOE : BulletAOE
{
    private float damageDuration;
    private float damageAmount;
    private Color associatedColor;
    public void Initialize(float dmgDuration, float dmg, Color _associatedColor)
    {
        damageDuration = dmgDuration;
        damageAmount = dmg;
        associatedColor = _associatedColor;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NPCManagerScript hitNPC))
        {
            hitNPC._stats.damageNumberColor = associatedColor;
            hitNPC._stats.AddDamageOverTime(damageDuration, damageAmount);
        }
    }


}
