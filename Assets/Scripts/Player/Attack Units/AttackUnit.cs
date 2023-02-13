using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackUnit : MonoBehaviour
{
    [Header("ATTACK UNIT PROPERTIES"), Space(2)]
    public AttackType attackType;
    [Range(0, 10)] public int resourceCost = 2;
    public LayerMask enemyLayerMask;
    public GameObject attackBulletPrefab;

    [Header("Attack Properties")]
    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;  // Delay between consequetive shots
    [Range(0.1f, 100f)] public float shootingRange = 10f;       // Range of the attack unit to fins enemies
    [Range(0.01f, 10f)] public float unitRefreshAfter = 2f;     // Dleay between unit searching for newer targets again
    private float timeSinceUnitRefresh = 0;

    [Header("Merging Properties")]
    public bool supportsCombining = false;
    public List<MergingCombinations> possibleCombinations = new();


    [Space(2), Header("READONLY")]
    [ReadOnly] public Transform targetTF;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public PlayerTower parentTower;
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new();
    [ReadOnly] public AttackUnitState currentUnitState;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    void Awake()
    {
        timeSinceUnitRefresh = unitRefreshAfter;
        currentUnitState = AttackUnitState.Idle;
    }
    public virtual void UpdateUnit()
    {
        RefreshTargetsList();
        UpdateTowerState();
        if (currentUnitState == AttackUnitState.Idle) TurretIdleAction();
        else if (currentUnitState == AttackUnitState.Attack) TurretAttackAction();
    }

    protected void UpdateTowerState()
    {
        //Switch Between States of turret 
        if (targetsInRange.Count > 0 && currentUnitState != AttackUnitState.Attack)
        {
            currentUnitState = AttackUnitState.Attack;
        }
        else if (targetsInRange.Count == 0 && currentUnitState != AttackUnitState.Idle)
        {
            currentUnitState = AttackUnitState.Idle;
        }
    }
    protected void RefreshTargetsList()
    {
        // Refresh the target
        timeSinceUnitRefresh += Time.deltaTime;
        if (unitRefreshAfter < timeSinceUnitRefresh)
        {
            timeSinceUnitRefresh = 0;

            targetTF = null;
            targetsInRange.Clear();

            Collider[] hitColliders = new Collider[5];

            int numTargetsFound = Physics.OverlapSphereNonAlloc(transform.position, shootingRange, hitColliders, enemyLayerMask);

            if (numTargetsFound > 0)
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider)
                    {
                        hitCollider.TryGetComponent(out NPCManagerScript target);
                        if (target) targetsInRange.Add(target);
                    }
                }
        }
    }

    protected virtual void TurretIdleAction()
    {

    }
    protected virtual void TurretAttackAction()
    {

        // Look for targets within shooting range
        if (targetTF == null)
        {
            float closestDistance = Mathf.Infinity;

            foreach (NPCManagerScript targetNPC in targetsInRange)
            {
                GameObject enemy;
                if (targetNPC != null)
                {
                    enemy = targetNPC.gameObjectSelf;

                    float distance = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distance < closestDistance && distance <= shootingRange)
                    {
                        closestDistance = distance;
                        targetTF = enemy.transform;
                    }
                }

            }
        }

        // Shoot at the target if found
        if (targetTF != null)
        {
            if (timeSinceLastAttack > delayBetweenShots)
            {
                timeSinceLastAttack = 0f;
                ShootAtTarget();
            }
            else timeSinceLastAttack += Time.deltaTime;
        }
    }

    protected virtual void ShootAtTarget()
    {

        // Create a new bullet
        GameObject bullet = Instantiate(attackBulletPrefab, transform.position + new Vector3(0, 0f, 0), transform.rotation);

        bullet.GetComponent<Bullet>().InitializeBullet(targetTF.position);

    }
}
