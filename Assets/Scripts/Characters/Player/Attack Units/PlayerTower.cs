using System.Collections.Generic;
using UnityEngine;
public class PlayerTower : PlayerUnitBase
{
    [Space(2), Header("PLAYER TOWER PROPERTIES"), Space(2)]
    public AttackType TowerAttackType;
    [Range(0, 10)] public int resourceCost = 2;

    [Space(2), Header("Merging Properties")]
    public bool supportsCombining = false;
    public List<MergingCombinations> possibleCombinations = new();

    [Header("ATTACK UNIT PROPERTIES")]
    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;      // Delay between consequetive shots
    [Range(0.1f, 100f)] public float shootingRange = 10f;           // Range of the attack unit to fins enemies
    [Range(0.01f, 10f)] public float unitRefreshAfter = 2f;         // Dleay between unit searching for newer targets again
    public GameObject attackBulletPrefab;
    public Transform turretMuzzleTF;
    public LayerMask enemyLayerMask;
    private float timeSinceUnitRefresh = 0;

    [Space(2), Header("READONLY")]
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new();
    [ReadOnly] public PlayerUnitDeploymentArea deployedAtArea;
    [ReadOnly] public TowerState currentTowerState;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public Transform targetTF;

    internal Stats _stats;



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    protected override void Awake()
    {
        base.Awake();
        _stats = GetComponent<Stats>();
        timeSinceUnitRefresh = unitRefreshAfter;
        currentTowerState = TowerState.Idle;
    }
    protected override void Start()
    {
        base.Start();
        deployedAtArea = GetComponentInParent<PlayerUnitDeploymentArea>();
    }

    private void Update()
    {
        UpdateUnit();
    }

    public void UpdateUnit()
    {
        RefreshTargetsList();
        UpdateTowerState();
        if (currentTowerState == TowerState.Idle) TurretIdleAction();
        else if (currentTowerState == TowerState.Attack) TurretAttackAction();
    }

    protected void UpdateTowerState()
    {
        //Switch Between States of turret 
        if (targetsInRange.Count > 0 && currentTowerState != TowerState.Attack)
        {
            currentTowerState = TowerState.Attack;
        }
        else if (targetsInRange.Count == 0 && currentTowerState != TowerState.Idle)
        {
            currentTowerState = TowerState.Idle;
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

            Collider[] hitColliders = new Collider[32];

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
        Vector3 bulletSpawnPos = turretMuzzleTF ? turretMuzzleTF.position : transform.position;
        // Create a new bullet
        GameObject bullet = Instantiate(attackBulletPrefab, bulletSpawnPos, transform.rotation);

        bullet.GetComponent<Bullet>().InitializeBullet(targetTF.position);

    }

    public void OnTowerDestroyed()
    {
        if (deployedAtArea) deployedAtArea.isAreaAvailable = false;
    }
}
