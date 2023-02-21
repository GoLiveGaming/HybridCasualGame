using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerTower : PlayerUnitBase
{
    [Space(2), Header("PLAYER TOWER PROPERTIES"), Space(2)]
    [Range(0, 10)] public int resourceCost = 2;
    [Range(0, 10)] public int constructionTime = 3;

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
    private bool isActive;
    private bool initialized = false;

    internal Stats _stats;



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    protected void OnDisable()
    {
        RemoveUnitFromMain();
    }
    protected void Start()
    {
        if (!initialized) Initialize();
    }

    private void Update()
    {
        UpdateUnit();
    }

    void Initialize()
    {
        isActive = false;
        _stats = GetComponent<Stats>();

        timeSinceUnitRefresh = unitRefreshAfter;
        currentTowerState = TowerState.Idle;
        deployedAtArea = GetComponentInParent<PlayerUnitDeploymentArea>();
        initialized = true;

        StartCoroutine(StartDeploymentSequence());
    }
    IEnumerator StartDeploymentSequence()
    {
        isActive = false;
        _stats.m_healthBar.fillAmount = 0;
        float elapsedTime = 0;
        float startValue = _stats.m_healthBar.fillAmount;
        float endValue = 1f;

        while (elapsedTime < constructionTime)
        {
            float t = elapsedTime / constructionTime;
            _stats.m_healthBar.fillAmount = Mathf.Lerp(startValue, endValue, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _stats.m_healthBar.fillAmount = endValue;
        AddUnitToMain();
        isActive = true;
    }

    public void UpdateUnit()
    {
        if (isActive)
        {
            RefreshTargetsList();
            UpdateTowerState();
            if (currentTowerState == TowerState.Idle) TurretIdleAction();
            else if (currentTowerState == TowerState.Attack) TurretAttackAction();
        }
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

    private void OnDestroy()
    {
        if (mainPlayerControl)
        {
            ParticleSystem deathParticle = Instantiate(mainPlayerControl.towerParticles[2], transform.position, Quaternion.identity);
            if(AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.TowerDestroyed);
            Destroy(deathParticle.gameObject, deathParticle.main.duration);
        }
    }
}
