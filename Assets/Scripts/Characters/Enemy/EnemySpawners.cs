using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawners : MonoBehaviour
{
    public static EnemySpawners Instance;

    [Header("Proximity Spawning"), Space(2)]
    [SerializeField] private bool spawnInProximity;
    [SerializeField, Range(0.1f, 10f)] private float proxSpawnradius = 5;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private NPCManagerScript[] ObjectsToSpawn;
    [SerializeField] private bool canSpawnEnemies;

    [SerializeField] private Transform enemiesParent;

    [SerializeField, Range(0.1f, 100f)] private float spawnInterval = 2f;
    [SerializeField, Range(0.1f, 100f)] private float spawnIntervalOffset = 2f;

    private float currentSpawnInterval;

    void OnDrawGizmosSelected()
    {

        if (spawnLocations.Length > 0)
        {
            foreach (Transform tf in spawnLocations)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(tf.position, proxSpawnradius);
            }
        }
        else
        {
            Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, proxSpawnradius);
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
       // if (spawnInProximity) StartCoroutine(SpawnEnemiesInProximity());
    }

    IEnumerator SpawnEnemiesInProximity()
    {
        while (canSpawnEnemies)
        {
            Vector3 spawnLocation;
            int spawnLocationIndex = Random.Range(0, spawnLocations.Length);
            int objectToSpawnIndex = Random.Range(0, ObjectsToSpawn.Length);

            if (spawnLocations.Length > 0)
            {
                if (spawnInProximity) spawnLocation = RandomNavSphere(spawnLocations[spawnLocationIndex].position, proxSpawnradius, -1);
                else spawnLocation = spawnLocations[spawnLocationIndex].position;
            }
            else
            {
                if (spawnInProximity) spawnLocation = RandomNavSphere(transform.position, proxSpawnradius, -1);
                else spawnLocation = transform.position;
            }
            Instantiate(ObjectsToSpawn[objectToSpawnIndex], spawnLocation, Quaternion.identity);

            currentSpawnInterval = spawnInterval + Random.Range(-spawnIntervalOffset, spawnIntervalOffset);

            yield return new WaitForSeconds(currentSpawnInterval);

        }
    }

    internal void SpawnEnemiesInWaves(EnemyTypes type, int objectSpawnPosIndex)
    {
        Vector3 spawnLocation;
        spawnLocation = RandomNavSphere(transform.position, proxSpawnradius, -1);
        int objectindex = ObjectToSpawnIndex(type);
        NPCManagerScript tempObj = Instantiate(ObjectsToSpawn[objectindex],spawnLocations[objectSpawnPosIndex].position, Quaternion.identity);
        tempObj.transform.SetParent(enemiesParent);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;


        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

        return navHit.position;
    }

    public int ObjectToSpawnIndex(EnemyTypes type)
    {
        for(int i = 0; i< ObjectsToSpawn.Length; i++)
        {
            if(ObjectsToSpawn[i].enemyType == type)
            {
                return i;
            }
        }

        return 0;
    
    }
}
