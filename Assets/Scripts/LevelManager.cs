using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // [Header("Level Data"), Space(2)]
    public LevelData[] levelData;
    internal bool isGameOver;
    int WaveIndexMain = 0;
    private void Start()
    {
        //try
        //{
            StartCoroutine(InstaniateEnemies(levelData[0].Waves[WaveIndexMain].enemyData[0].TimeInterval));
        //}
        //catch
        //{
        //    Debug.LogError("There are no levels");
        //}
        
    }

    IEnumerator InstaniateEnemies(int time)
    {
        //while (!isGameOver)
        //{
            UIManager.Instance.ShowText("Wave " + (levelData[0].Waves[WaveIndexMain].waveNum));
            SpawnEnemies(WaveIndexMain);
            WaveIndexMain++;
            yield return new WaitForSeconds(time);
            if (levelData[0].Waves.Length <= WaveIndexMain)
            {
                isGameOver = true;
            }
            else
            {
                StartCoroutine(InstaniateEnemies(levelData[0].Waves[WaveIndexMain].enemyData[0].TimeInterval));
            }
       // }
    }

    void SpawnEnemies(int waveIndex)
    {
        for (int i = 0; i < levelData[0].Waves[waveIndex].enemyData.Length; i++)
        {
            for (int j = 0; j < levelData[0].Waves[waveIndex].enemyData[i].enemyCount; j++)
            {
                EnemySpawners.Instance.SpawnEnemiesInWaves(levelData[0].Waves[waveIndex].enemyData[i].enemyType, levelData[0].Waves[waveIndex].enemyData[i].SpawnLocation);
            }
        }
    }


}

[Serializable]
public class LevelData
{
    public WaveData[] Waves;
}

[Serializable]
public class WaveData
{
    public int waveNum;
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
