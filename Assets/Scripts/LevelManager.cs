using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    // [Header("Level Data"), Space(2)]
    public LevelData[] levelData;

    internal bool isGameOver;
    int WaveIndexMain = 0;
    [ReadOnly] public int levelNum = 0;
    [ReadOnly] public int deadEnemiesCount = 0;
    [ReadOnly] public int maxEnemyCount;
    [ReadOnly] public int spawnedEnemyCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIManager.Instance.loadingPanel.gameObject.SetActive(true);
        levelNum = PlayerPrefs.GetInt("CurrentLevel");
        maxEnemyCount = levelData[levelNum].totalEnemies;
        deadEnemiesCount = levelData[levelNum].totalEnemies;
        UIManager.Instance.enemiesCountTxt.text = levelData[levelNum].totalEnemies.ToString();
        StartCoroutine(LoadingScene());
        InstantiateAllEnemiesAtStart();
    }

    IEnumerator LoadingScene()
    {
        while (maxEnemyCount > spawnedEnemyCount)
        {
            UIManager.Instance.loadingfiller.fillAmount = spawnedEnemyCount / maxEnemyCount;
            yield return null;
        }
        yield return new WaitWhile(() => maxEnemyCount > spawnedEnemyCount);
        UIManager.Instance.loadingPanel.gameObject.SetActive(false);
        if(AudioManager.Instance)AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.LevelStart);
        StartCoroutine(SpawnEnemiesInIntervels(levelData[levelNum].Waves[WaveIndexMain].enemyData[0].TimeInterval));
    }

    

    void InstantiateAllEnemiesAtStart()
    {
        for(int i =0; i < levelData[levelNum].Waves.Length; i++)
        {
            for (int j = 0; j < levelData[levelNum].Waves[i].enemyData.Length; j++)
            {
                for (int k = 0; k < levelData[levelNum].Waves[i].enemyData[j].enemyCount; k++)
                {
                    EnemySpawners.Instance.SpawnEverythingAtStart(levelData[levelNum].Waves[i].enemyData[j].enemyType, levelData[levelNum].Waves[i].enemyData[j].SpawnLocation);
                    spawnedEnemyCount++;
                }
            }
        }
    }

    IEnumerator SpawnEnemiesInIntervels(int time)
    {
        //while (!isGameOver)
        //{
            if(levelData[levelNum].Waves.Length > levelData[levelNum].Waves[WaveIndexMain].waveNum)
            UIManager.Instance.WaveBouncyText(levelData[levelNum].Waves[WaveIndexMain].waveNum, levelData[levelNum].Waves[WaveIndexMain].totalEniemies, levelData[levelNum].Waves[WaveIndexMain+1].waveNum, levelData[levelNum].Waves[WaveIndexMain+1].totalEniemies);
            else
            UIManager.Instance.WaveBouncyText(levelData[levelNum].Waves[WaveIndexMain].waveNum, levelData[levelNum].Waves[WaveIndexMain].totalEniemies, 0,0);
            SpawnEnemies(WaveIndexMain);
            WaveIndexMain++;
            yield return new WaitForSeconds(time);
            if (levelData[levelNum].Waves.Length <= WaveIndexMain)
            {
                isGameOver = true;
            }
            else
            {
                StartCoroutine(SpawnEnemiesInIntervels(levelData[levelNum].Waves[WaveIndexMain].enemyData[0].TimeInterval));
                yield break;
            }
       // }
    }
    int aa = 0;
    void SpawnEnemies(int waveIndex)
    {
        for (int i = 0; i < levelData[levelNum].Waves[waveIndex].enemyData.Length; i++)
        {
            for (int j = 0; j < levelData[levelNum].Waves[waveIndex].enemyData[i].enemyCount; j++)
            {
                //if(aa < -1)
                EnemySpawners.Instance.SpawnEnemiesInWaves(levelData[levelNum].Waves[waveIndex].enemyData[i].enemyType);
                aa++;
            }
        }
    }


}

[Serializable]
public class LevelData
{
    public int totalEnemies;
    public WaveData[] Waves;
}

[Serializable]
public class WaveData
{
    public int waveNum;
    public int totalEniemies;
    public EnemyData[] enemyData;
}

[Serializable]
public class EnemyData
{
    public int SpawnLocation;
    public EnemyTypes enemyType;
    public int enemyCount;
    public int TimeInterval;
}

[Serializable]
public enum EnemyTypes
{
    Melee,
    Heavies,
    Ranged, 
    RangedBig
}
