using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerTower;

[Serializable]
public class AttackUnit : MonoBehaviour
{
    [Header("Attack Unit Properties"), Space(2)]
    public AttackType attackUnitType;

    [SerializeField, ReadOnly] private Bullets[] _attackBullets;

    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;
    [ReadOnly][Range(0.01f, 10f)] private float baseDelayBetweenShots;
    [Range(0.1f, 100f)] public float shootingRange = 10f;
    [Range(0.01f, 10f)] public float unitRefreshAfter = 2f;
    private float timeSinceUnitRefresh = 0;

    [Space(2), Header("ReadOnly")]
    [ReadOnly] public Transform targetTF;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new List<NPCManagerScript>();
    [ReadOnly] public PlayerTower parentTower;


    private AttackType oldAttackType;
    private int currentAttackBulletIndex = 0;
    public GameObject attackBulletPrefab
    {
        get
        {
            if (_attackBullets.Length == 0) if (parentTower) _attackBullets = parentTower.mainPlayerControl.attackBulletVariants;
            if (oldAttackType == attackUnitType) return _attackBullets[currentAttackBulletIndex].bulletPrefab;
            else
            {
                oldAttackType = attackUnitType;
                int index = 0;
                foreach (Bullets bullet in _attackBullets)
                {
                    if (bullet.associatedAttack == attackUnitType)
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
    private void Awake()
    {
        baseDelayBetweenShots = delayBetweenShots;
        timeSinceUnitRefresh = unitRefreshAfter;
        parentTower = GetComponent<PlayerTower>();
    }

    private void Start()
    {

    }
    public void UpdateUnit()
    {
        RefreshTargetsList();
        if (parentTower.towerState == TowerState.Idle) TurretIdleAction();
        else if (parentTower.towerState == TowerState.Attack) TurretAttackAction();
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



    public void UpdateDelayBetweenShots(int towerLevel)
    {
        delayBetweenShots -= parentTower.upgradeDecreasedShootDelay;
        delayBetweenShots = Mathf.Clamp(delayBetweenShots, 0.01f, 10f);
    }

}
