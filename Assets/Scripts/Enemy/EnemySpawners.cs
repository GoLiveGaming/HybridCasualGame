using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawners : MonoBehaviour
{
    [SerializeField] private NPCManagerScript targetEnemy;
    [SerializeField] private bool canSpawnEnemies;
    [SerializeField, Range(0.1f, 100f)] private float spawnInterval = 2f;

    void Start()
    {
        // InvokeRepeating("EnemySpawningVoid", 0, spawnInterval);
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (canSpawnEnemies)
        {
            NPCManagerScript Obj = Instantiate(targetEnemy, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);

        }
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
