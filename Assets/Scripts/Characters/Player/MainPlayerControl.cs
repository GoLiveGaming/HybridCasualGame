using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Space(2), Header("ATTACK UNITS"), Space(2)]
    public PlayerUnit[] allPlayerUnits;

    public ParticleSystem[] towerParticles;
    public ParticleSystem[] enemyParticles;

    [Space(2), Header("UNIT UPGRADES"), Space(2)]
    [SerializeField] private LayerMask deployAreaLayer;
    [SerializeField] private UnitUpgradesButton upgradeButtonPrefab;

    [Header("RESOURCE METER"), Space(2)]
    [Range(1, 20)] public float maxResources = 10;
    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f;

    [Space(2), Header("SCORE DATA"), Space(2)]
    public ScoringData scoringData;
    [Header("Point Allotment")]
    [SerializeField] private int towerPlacedScore = 10;
    [SerializeField] private int towerUpgradedScore = 20;
    [SerializeField] private int towerDestroyedScore = -5;
    [SerializeField] private int mainTowerHealthLostScore = -5;
    [SerializeField] private int enemyWaveSurvivedScore = 50;

    [Space(2), Header("READONLY")]
    [ReadOnly, Range(1, 20)] public float currentResourcesCount = 10;
    [ReadOnly] public List<PlayerUnitBase> activePlayerTowersList = new();
    [ReadOnly] public PlayerMainTower mainPlayerTower;
    [ReadOnly] public PlayerDataManager _dataManager;
    [ReadOnly] public bool isRecharging = false;
    private UIManager uiManager;
    private Camera mainCamera;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;

        uiManager = UIManager.Instance;
        _dataManager = PlayerDataManager.Instance;
    }
    void Update()
    {
        HandleResources();
        HandleUnitUpgrades();
    }

    public PlayerUnit GetPlayerUnit(AttackType unitType)
    {
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            if (unit.unitType == unitType)
                return unit;
        }
        return null;
    }

    #region SCORE TRACKING
    public void AddEnemiesKilledData(EnemyTypes enemyType)
    {
        bool flag = false;
        foreach (EnemiesKilledData data in scoringData.enemiesKilledData)
        {
            if (data.enemyType == enemyType)
            {
                data.numKilled++;
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            Debug.LogError(scoringData.enemiesKilledData + " has not been assigned any Data.");
        }
    }
    public int CalculateTotalScore()
    {
        int totalScore = 0;

        foreach (var data in scoringData.enemiesKilledData)
        {
            totalScore += data.killScorePoint * data.numKilled;
        }

        totalScore += TowersPlacedNum * towerPlacedScore;
        totalScore += TowersUpgradedNum * towerUpgradedScore;
        totalScore += TowersDestroyedNum * towerDestroyedScore;
        totalScore += EnemyWavesCompletedNum * enemyWaveSurvivedScore;
        totalScore += MainTowerHealthLostNum * mainTowerHealthLostScore;

        return totalScore;
    }

    public int TotalEnemiesKilledNum
    {
        get
        {
            int num = 0;
            foreach (EnemiesKilledData data in scoringData.enemiesKilledData)
            {
                num += data.numKilled;
            }
            return num;
        }
    }
    public int TowersPlacedNum { get { return scoringData.TowersPlaced; } set { scoringData.TowersPlaced = value; } }
    public int TowersUpgradedNum { get { return scoringData.TowersUpgraded; } set { scoringData.TowersUpgraded = value; } }
    public int TowersDestroyedNum { get { return scoringData.TowersDestroyed; } set { scoringData.TowersDestroyed = value; } }
    public int EnemyWavesCompletedNum { get { return scoringData.EnemyWavesSurvived; } set { scoringData.EnemyWavesSurvived = value; } }
    public int MainTowerHealthLostNum { get { return scoringData.MainTowerHealthLostNum = (int)mainPlayerTower.GetComponent<Stats>().m_MaxHealth - (int)mainPlayerTower.GetComponent<Stats>().Health; } }


    #endregion

    #region UPGRADES MANAGEMENT
    private void HandleUnitUpgrades()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if the touch was on a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Cast a ray from the touch position in the specified direction
            Ray ray = mainCamera.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200, deployAreaLayer))
            {
                if (hit.transform.TryGetComponent(out PlayerUnitDeploymentArea area))
                {
                    if (!area.HasDeployedUnit)
                    {
                        uiManager.unitUpgradesPanel.SetActive(false);
                        return;
                    }

                    StartUpgradesProcess(area);
                }
            }
            else
            {
                uiManager.unitUpgradesPanel.SetActive(false);
            }
        }
    }

    private void StartUpgradesProcess(PlayerUnitDeploymentArea area)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(area.transform.position);

        foreach (Transform tf in uiManager.unitUpgradesPanel.transform)
        {
            Destroy(tf.gameObject);
        }
        foreach (MergingCombinations existingUnitCombination in area.deployedTower.possibleCombinations)
        {
            if (!_dataManager.IsAttackTypeUnlocked(existingUnitCombination.toYield)) continue;

            UnitUpgradesButton upgradesButton = Instantiate(upgradeButtonPrefab, uiManager.unitUpgradesPanel.transform);

            upgradesButton.InitializeButton(GetPlayerUnit(existingUnitCombination.toYield), area);
        }

        RectTransform rectTransform = uiManager.unitUpgradesPanel.GetComponent<RectTransform>();
        rectTransform.position = screenPosition + new Vector3(0, 400, 0);
        uiManager.unitUpgradesPanel.SetActive(true);
    }

    #endregion

    #region RESOURCE MANAGEMENT
    public void HandleResources()
    {
        if (currentResourcesCount < maxResources && !isRecharging)
        {
            StartCoroutine(RechargeResource());
        }
    }
    IEnumerator RechargeResource()
    {
        if (uiManager.resourceMeterAnimator) uiManager.resourceMeterAnimator.SetBool("Full", false);
        isRecharging = true;
        if (currentResourcesCount == 0 && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaOut);


        while (currentResourcesCount < maxResources)
        {
            yield return null;
            currentResourcesCount += resourceRechargeRate * Time.deltaTime;
            uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
            uiManager.resourcesCount.text = Mathf.RoundToInt(currentResourcesCount).ToString();

        }
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
        uiManager.resourcesCount.text = Mathf.RoundToInt(currentResourcesCount).ToString();
        if (currentResourcesCount == maxResources && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaFull);
        if (uiManager.resourceMeterAnimator) uiManager.resourceMeterAnimator.SetBool("Full", true);
        isRecharging = false;
    }

    public void RemoveResource(int amount)
    {
        currentResourcesCount -= amount;
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
    }

    #endregion
}

[Serializable]
public class ScoringData
{
    public List<EnemiesKilledData> enemiesKilledData = new();

    public int TowersPlaced = 0;
    public int TowersUpgraded = 0;
    public int TowersDestroyed = 0;
    public int MainTowerHealthLostNum = 0;

    public int EnemyWavesSurvived = 0;
}
[Serializable]
public class EnemiesKilledData
{
    public EnemyTypes enemyType;
    public int killScorePoint = 0;
    [ReadOnly] public int numKilled = 0;
}