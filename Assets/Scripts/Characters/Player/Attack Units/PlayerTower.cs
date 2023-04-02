using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static UnityEngine.Rendering.DebugUI;

public class PlayerTower : PlayerUnitBase
{
    [Space(2), Header("PLAYER TOWER PROPERTIES"), Space(2)]
    [SerializeField] private AttackType attackType;
    [Range(0, 10)] public int constructionTime = 3;
    [SerializeField] private GameObject incompleteTowerObject;
    [SerializeField] private GameObject completedTowerObject;

    [Header("ATTACK UNIT PROPERTIES")]

    [Range(0.01f, 10f)] public float delayBetweenShots = 0.5f;      // Delay between consequetive shots
    [Range(0.1f, 100f)] public float shootingRange = 10f;           // Range of the attack unit to find enemies
    [Range(0.01f, 10f)] public float unitRefreshAfter = 2f;         // Dleay between unit searching for newer targets again
    public GameObject attackBulletPrefab;
    public Transform turretMuzzleTF;
    public LayerMask enemyLayerMask;


    [Space(2), Header("READONLY")]

    [ReadOnly] public PlayerTowerUI playerTowerUI;
    [ReadOnly] public PlayerUnit playerUnitProperties;
    [ReadOnly] public List<NPCManagerScript> targetsInRange = new();
    [ReadOnly] public PlayerUnitDeploymentArea deployedAtArea;
    [ReadOnly] private float timeSinceUnitRefresh = 0;
    [ReadOnly] public float timeSinceLastAttack = 0f;
    [ReadOnly] public Transform targetTF;
    private bool isActive;

    public Stats _stats;
    public UIManager _uiManager;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    protected void OnEnable()
    {
        SpawnParticles(0, -90);
    }
    protected void OnDisable()
    {
        RemoveUnitFromMain();
        if (_mainPlayerControl)
        {
            SpawnParticles(2, -90);
        }
        if (playerTowerUI) Destroy(playerTowerUI.gameObject);
    }

    private void Update()
    {
        UpdateUnit();
    }

    public void Initialize(PlayerUnit unit)
    {
        isActive = false;
        playerUnitProperties = unit;

        _uiManager = UIManager.Instance;
        _mainPlayerControl = MainPlayerControl.Instance;

        _stats = GetComponent<Stats>();

        deployedAtArea = GetComponentInParent<PlayerUnitDeploymentArea>();


        _uiManager.ShowFloatingResourceRemovedUI("-" + playerUnitProperties.resourceCost.ToString(), transform.position);
        playerTowerUI = Instantiate(_mainPlayerControl.playerTowerUIPrefab, _uiManager.rootCanvas.transform);
        playerTowerUI.InitializeUI(this, playerUnitProperties.statsUISprite);


        _stats.InitializeStats();


        timeSinceUnitRefresh = unitRefreshAfter;
        StartCoroutine(StartDeploymentSequence());
    }
    IEnumerator StartDeploymentSequence()
    {

        BuildTower(false);
        isActive = false;

        playerTowerUI.HealthBarImage.fillAmount = 0;
        float elapsedTime = 0;
        float startValue = 0;
        float endValue = 1f;

        while (elapsedTime < constructionTime)
        {
            float t = elapsedTime / constructionTime;
            playerTowerUI.HealthBarImage.fillAmount = Mathf.Lerp(startValue, endValue, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerTowerUI.HealthBarImage.fillAmount = endValue;
        AddUnitToMain();
        SpawnParticles(3, -90);
        BuildTower(true);
        isActive = true;
    }

    private void BuildTower(bool complete)
    {
        if (!incompleteTowerObject || !completedTowerObject) return;
        if (complete)
        {
            incompleteTowerObject.SetActive(false);
            completedTowerObject.SetActive(true);
        }
        else
        {
            incompleteTowerObject.SetActive(true);
            completedTowerObject.SetActive(false);
        }
    }

    public void UpdateUnit()
    {
        if (!isActive) return;

        RefreshTargetsList();
        TurretAttackAction();
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
                    enemy = targetNPC.GameObjectSelf;

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

    void SpawnParticles(int particleIndex, int rotation)
    {
        ParticleSystem particleTemp = Instantiate(MainPlayerControl.Instance.towerParticles[particleIndex], transform.position, Quaternion.Euler(rotation, 0f, 0f));
        Destroy(particleTemp.gameObject, particleTemp.main.duration);
    }
}
