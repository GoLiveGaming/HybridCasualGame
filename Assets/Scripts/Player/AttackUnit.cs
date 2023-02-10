using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackUnit : MonoBehaviour
{
    [Header("ATTACK UNIT PROPERTIES"), Space(2)]
    public AttackType attackType;
    public GameObject attackUnitVisualPrefab;
    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;
    [Range(0.1f, 100f)] public float shootingRange = 10f;
    [Range(0.01f, 10f)] public float unitRefreshAfter = 2f;
    public bool supportsCombining = false;
    public List<CombinationRecipe> possibleCombinations = new List<CombinationRecipe>();

    private float timeSinceUnitRefresh = 0;

    [Space(2), Header("ReadOnly")]
    [ReadOnly] public Transform targetTF;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public PlayerTower parentTower;
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new List<NPCManagerScript>();
    [ReadOnly] public AttackUnitState currentUnitState;
    [SerializeField, ReadOnly] private Bullets[] _attackBullets;

    //Get AttackBullets
    private AttackType oldAttackType;
    private int currentAttackBulletIndex = 0;
    public GameObject attackBulletPrefab
    {
        get
        {
            if (_attackBullets.Length == 0) if (parentTower) _attackBullets = parentTower.mainPlayerControl.allAttackBullets;
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


    void Awake()
    {
        timeSinceUnitRefresh = unitRefreshAfter;
        currentUnitState = AttackUnitState.Idle;
    }
    public void UpdateUnit()
    {
        RefreshTargetsList();
        UpdateTowerState();
        if (currentUnitState == AttackUnitState.Idle) TurretIdleAction();
        else if (currentUnitState == AttackUnitState.Attack) TurretAttackAction();
    }

    private void UpdateTowerState()
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
    void RefreshTargetsList()
    {
        // Refresh the target
        timeSinceUnitRefresh += Time.deltaTime;
        if (unitRefreshAfter < timeSinceUnitRefresh)
        {
            targetTF = null;
            targetsInRange.Clear();
            timeSinceUnitRefresh = 0;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("TargetEnemy"))
                {
                    hitCollider.TryGetComponent(out NPCManagerScript target);
                    if (target) targetsInRange.Add(target);
                }
            }
        }
    }

    private void TurretIdleAction()
    {

    }
    private void TurretAttackAction()
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
    void ShootAtTarget()
    {
        GameObject bullet = Instantiate(attackBulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Bullet>().initializeBullet(targetTF);
    }
}
