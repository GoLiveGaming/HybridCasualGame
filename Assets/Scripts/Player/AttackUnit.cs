using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackUnit : MonoBehaviour
{
    [Header("Attack Unit Properties"), Space(2)]
    public AttackType attackType;

    [SerializeField] private Bullets[] _attackBullets;

    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;
    [Range(0.1f, 100f)] public float shootingRange = 10f;

    [Space(2), Header("ReadOnly")]
    [ReadOnly] public Transform targetTF;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new List<NPCManagerScript>();


    private AttackType oldAttackType;
    private int currentAttackBulletIndex = 0;
    public GameObject attackBulletPrefab
    {
        get
        {
            if (oldAttackType == attackType) return _attackBullets[currentAttackBulletIndex].bulletPrefab;
            else
            {
                oldAttackType = attackType;
                int index = 0;
                foreach (Bullets bullet in _attackBullets)
                {
                    if (bullet.associatedAttack == attackType)
                    {
                        currentAttackBulletIndex = index;
                        return bullet.bulletPrefab;
                    }
                    index++;
                }

                Debug.LogError("No Bullets Assigned to " + this);
                return null;
            }
        }
    }
    [Serializable]
    public class Bullets
    {
        public GameObject bulletPrefab;
        public AttackType associatedAttack;
    }
}
