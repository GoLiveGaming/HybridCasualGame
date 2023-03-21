using GameAnalyticsSDK;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("ENEMY SPAWNING"), Space(2)]
    [SerializeField] private NPCManagerScript[] ObjectsToSpawn;
    [SerializeField] private Transform[] spawnLocations;

    [Header("Enemy Spawning Rules"), Space(2)]
    public LevelData[] levelData;
    public Transform enemiesParent;
    [SerializeField, Range(0.1f, 10f)] private float spawnProximityRadius = 5;

    [ReadOnly] public int levelNum = 0;
    [ReadOnly] public int currentWaveIndex = 0;
    [ReadOnly] public int totalEnemiesInLevel = 0;
    [ReadOnly] public int spawnedEnemyCount = 0;
    [ReadOnly] public int deadEnemiesCount = 0;



    readonly List<NPCManagerScript> spawnList = new();

    public int AliveEnemiesLeft
    {
        get { return totalEnemiesInLevel - deadEnemiesCount; }
    }

    private UIManager uiManager;
    private PlayerDataManager playerDataManager;
    private MainPlayerControl _mainPlayerControl;


    void OnDrawGizmos()
    {

        if (spawnLocations.Length > 0)
        {
            foreach (Transform tf in spawnLocations)
            {
                if (tf != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(tf.position, spawnProximityRadius);
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
        playerDataManager = PlayerDataManager.Instance;
        _mainPlayerControl = MainPlayerControl.Instance;

        InitializeEnemyData();

        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.LevelStart);

        StartLoadingSequence();
    }

    void StartLoadingSequence()
    {
        StartCoroutine(SpawnEnemiesInInterval());
    }

    #region ENEMY SPAWNING

    void InitializeEnemyData()
    {
        levelNum = 0;
        currentWaveIndex = 0;
        totalEnemiesInLevel = 0;
        spawnedEnemyCount = 0;
        deadEnemiesCount = 0;

        levelNum = playerDataManager.SelectedLevelIndex;

        Debug.Log("Loading Level: " + levelNum +
            " enemies data. Correct enemies data will only be loaded when game is started from HomeScreen.");

        string eventName = "Level_0" + (levelNum);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, eventName);

        foreach (WaveData data in levelData[levelNum].waves)
        {
            foreach (EnemyData i in data.enemyData)
            {
                totalEnemiesInLevel += i.enemyCount;
            }
        }

    }
    IEnumerator SpawnEnemiesInInterval()
    {

        while (currentWaveIndex < levelData[levelNum].waves.Length)
        {
            int time = levelData[levelNum].waves[currentWaveIndex].currentWaveWaitTime;
            while (time >= 0)
            {
                uiManager.nextWaveTimer.text = time.ToString();
                yield return new WaitForSeconds(1);
                time--;
            }
            SpawnEnemyWave();
            uiManager.ShowNewWaveInfo(levelData[levelNum].waves[currentWaveIndex]);

            _mainPlayerControl.EnemyWavesCompletedNum++;

            currentWaveIndex++;

            if (currentWaveIndex >= levelData[levelNum].waves.Length)
            {
                uiManager.nextWaveTimer.transform.parent.gameObject.SetActive(false);
            }

            yield return null;
        }
    }
    void SpawnEnemyWave()
    {
        foreach (EnemyData enemyData in levelData[levelNum].waves[currentWaveIndex].enemyData)
        {
            for (int i = 0; i < enemyData.enemyCount; i++)
            {
                SpawnEnemy(enemyData.enemyType, enemyData.spawnLocation);
                spawnedEnemyCount++;

            }
        }
    }

    public Transform GetSpawnTransform(int locationIndex)
    {
        return spawnLocations[locationIndex];
    }
    private void SpawnEnemy(EnemyTypes type, int objectSpawnPosIndex)
    {
        int objectindex = ObjectToSpawnIndex(type);
        NPCManagerScript tempObj = Instantiate(ObjectsToSpawn[objectindex],
            RandomNavSphere(spawnLocations[objectSpawnPosIndex].position, spawnProximityRadius, -1),
            Quaternion.identity);

        tempObj.Initialize();
        tempObj.transform.SetParent(enemiesParent);
        spawnList.Add(tempObj);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;


        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

        return navHit.position;
    }

    private int ObjectToSpawnIndex(EnemyTypes type)
    {
        for (int i = 0; i < ObjectsToSpawn.Length; i++)
        {
            if (ObjectsToSpawn[i].enemyType == type)
            {
                return i;
            }
        }
        return 0;
    }

    #endregion
}


[Serializable]
public class LevelData
{
    public WaveData[] waves;
}

[Serializable]
public class WaveData
{
    public int currentWaveWaitTime;
    public EnemyData[] enemyData;
}

[Serializable]
public class EnemyData
{
    public EnemyTypes enemyType;
    public int enemyCount;
    public int spawnLocation;
}



