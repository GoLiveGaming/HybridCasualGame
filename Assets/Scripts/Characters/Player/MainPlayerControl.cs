using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Space(2), Header("PLAYER ATTACK UNITS"), Space(2)]
    public PlayerUnit[] allPlayerUnits;

    public ParticleSystem[] towerParticles;
    public PlayerTowerUI playerTowerUIPrefab;
    public PlayerTowerMainUI playerTowerMainUIPrefab;

    [Space(2), Header("UNIT UPGRADES"), Space(2)]
    [SerializeField]
    private LayerMask deployAreaLayer;

    [SerializeField] private UnitUpgradesButton upgradeButtonPrefab;

    [Space(2), Header("ENEMY DATA"), Space(2)]
    [SerializeField]
    private EnemyData[] allEnemyData;

    [SerializeField] private ParticleSystem[] enemyParticles;

    [Header("RESOURCE METER"), Space(2)]
    [Range(1, 20)]
    public float maxResources = 10;

    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f;

    [Space(2), Header("SCORE DATA"), Space(2)]
    public ScoringData scoringData;

    [SerializeField, ReadOnly] private int totalScore;

    [Header("Point Allotment")]
    [SerializeField] private int towerPlacedScore = 10;
    [SerializeField] private int towerUpgradedScore = 20;
    [SerializeField] private int towerDestroyedScore = -5;
    [SerializeField] private int enemyWaveSurvivedScore = 50;
    [SerializeField] private int mainTowerHealthLostScore = -5;


    [Space(2), Header("READONLY")]
    [ReadOnly, Range(1, 20)]
    public float currentResourcesCount = 10;

    [ReadOnly] public List<PlayerUnitBase> activePlayerTowersList = new();
    [ReadOnly] public PlayerTowerMain _mainPlayerTower;
    [ReadOnly] public PlayerDataManager _dataManager;
    [ReadOnly] public bool isRecharging = false;
    private UIManager _uiManager;
    private static readonly int Full = Animator.StringToHash("Full");


    public EnemyData[] AllEnemyData
    {
        get { return allEnemyData; }
    }

    public ParticleSystem[] EnemyParticles
    {
        get { return enemyParticles; }
    }

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
    }

    void Start()
    {
        _uiManager = UIManager.Instance;
        _dataManager = PlayerDataManager.Instance;
    }

    void Update()
    {
        HandleResources();
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
    public PlayerUnit CanUnitsBeMerged(PlayerUnit unit01, PlayerUnit unit02)
    {
        if (unit02.supportsCombining)
        {
            foreach (MergingCombinations existingUnitCombination in unit02.possibleCombinations)
            {
                if (existingUnitCombination.combinesWith == unit01.unitType)
                {
                    PlayerUnit combinedUnit = GetPlayerUnit(existingUnitCombination.combinesWith);

                    if (!IsAttackTypeUnlocked(combinedUnit.unitType)) break;
                    Debug.Log("Upgrading to: " + combinedUnit);
                    return combinedUnit;
                }
            }
        }
        Debug.Log("No possible merge combinations found for: " + unit01.unitType);
        return null;
    }
    public bool IsAttackTypeUnlocked(AttackType type)
    {
        foreach (PlayerAttacksData data in _dataManager.PlayerData.AllAttackTypesData)
        {
            if (data.AttackType == type)
            {
                return data.isUnlocked > 0;
            }
        }

        return false;
    }

    public bool UnlockAttack(AttackType attackType)
    {
        return _dataManager.UnlockAttackType(attackType);
    }

    #region SCORE TRACKING

    public void AddEnemiesKilledData(EnemyTypes enemyType)
    {
        var flag = false;
        foreach (var data in scoringData.enemiesKilledData.Where(data => data.enemyType == enemyType))
        {
            data.numKilled++;
            flag = true;
            break;
        }

        if (!flag)
        {
            for (int i = 0; i < allEnemyData.Length; i++)
            {
                if (allEnemyData[i].enemyPrefab.enemyType == enemyType)
                {
                    var npc = allEnemyData[i].enemyPrefab;
                    EnemiesKilledData data = new()
                    {
                        enemyType = npc.enemyType,
                        numKilled = 1
                    };

                    scoringData.enemiesKilledData.Add(data);
                    break;
                }
            }
        }
    }

    public int CalculateTotalScore()
    {
        totalScore = 0;

        foreach (var data in scoringData.enemiesKilledData)
        {
            foreach (var enemyData in allEnemyData)
            {
                if (data.enemyType != enemyData.enemyPrefab.enemyType)
                {
                    continue;
                }

                totalScore += (int)enemyData.killedScore * data.numKilled;
                break;
            }
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
        get { return scoringData.enemiesKilledData.Sum(data => data.numKilled); }
    }

    public int TowersPlacedNum
    {
        get { return scoringData.towersPlaced; }
        set { scoringData.towersPlaced = value; }
    }

    public int TowersUpgradedNum
    {
        get { return scoringData.towersUpgraded; }
        set { scoringData.towersUpgraded = value; }
    }

    public int TowersDestroyedNum
    {
        get { return scoringData.towersDestroyed; }
        set { scoringData.towersDestroyed = value; }
    }

    public int EnemyWavesCompletedNum
    {
        get { return scoringData.enemyWavesSurvived; }
        set { scoringData.enemyWavesSurvived = value; }
    }

    public int MainTowerHealthLostNum
    {
        get { return scoringData.mainTowerHealthLostNum; }
        set { scoringData.mainTowerHealthLostNum = value; }
    }

    #endregion

    #region RESOURCE MANAGEMENT

    private void HandleResources()
    {
        if (currentResourcesCount < maxResources && !isRecharging)
        {
            StartCoroutine(RechargeResource());
        }
    }

    private IEnumerator RechargeResource()
    {
        if (_uiManager.resourceMeterAnimator) _uiManager.resourceMeterAnimator.SetBool(Full, false);
        isRecharging = true;
        if (currentResourcesCount == 0 && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaOut);


        while (currentResourcesCount < maxResources)
        {
            yield return null;
            currentResourcesCount += resourceRechargeRate * Time.deltaTime;
            _uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
            _uiManager.resourcesCountText.text = Mathf.RoundToInt(currentResourcesCount).ToString();
        }

        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        _uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
        _uiManager.resourcesCountText.text = Mathf.RoundToInt(currentResourcesCount).ToString();
        if (Math.Abs(currentResourcesCount - maxResources) < 0.1f && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaFull);
        if (_uiManager.resourceMeterAnimator) _uiManager.resourceMeterAnimator.SetBool(Full, true);
        isRecharging = false;
    }

    public void RemoveResource(int amount)
    {
        currentResourcesCount -= amount;
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        _uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
    }

    #endregion
}

[Serializable]
public class ScoringData
{
    public List<EnemiesKilledData> enemiesKilledData = new();

    public int towersPlaced = 0;
    public int towersUpgraded = 0;
    public int towersDestroyed = 0;
    public int enemyWavesSurvived = 0;
    public int mainTowerHealthLostNum = 0;
}

[Serializable]
public class EnemiesKilledData
{
    public EnemyTypes enemyType;
    public int numKilled = 0;
}

[Serializable]
public class EnemyData
{
    public NPCManagerScript enemyPrefab;
    public float killedScore = 1;
}