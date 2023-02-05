using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawners : MonoBehaviour
{
    public NPCManagerScript targetEnemy;
    public int spawnInterval;
    int timespan = 2;
    // Start is called before the first frame update
    void Start()
    {
        // InvokeRepeating("EnemySpawningVoid", 0, spawnInterval);
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(timespan);
        NPCManagerScript Obj = GameObject.Instantiate<NPCManagerScript>(targetEnemy);
        Obj.transform.localPosition = transform.position;
        Obj.gameObject.SetActive(true);
        timespan = Random.Range(10, 15);
        StartCoroutine(SpawnEnemies());
    }
    void EnemySpawningVoid()
    {
       // spawnInterval = Random.Range(10, 15);
        NPCManagerScript Obj = GameObject.Instantiate<NPCManagerScript>(targetEnemy);
        Obj.transform.localPosition = transform.position;
        Obj.gameObject.SetActive(true);
      //  Obj.transform.SetParent(neighbourhoodsContainer, false);
    }
}
