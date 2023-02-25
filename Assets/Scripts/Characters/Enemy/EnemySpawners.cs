using System;
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

    [SerializeField] internal Transform enemiesParent;

    [SerializeField, Range(0.1f, 100f)] private float spawnInterval = 2f;
    [SerializeField, Range(0.1f, 100f)] private float spawnIntervalOffset = 2f;

   // public Dictionary<EnemyTypes, Queue<NPCManagerScript>> spawnDictionary = new Dictionary<EnemyTypes, Queue<NPCManagerScript>>();
    List<NPCManagerScript> spawnList = new List<NPCManagerScript>();

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

    internal void SpawnEverythingAtStart(EnemyTypes type, int objectSpawnPosIndex)
    {
        int objectindex = ObjectToSpawnIndex(type);
        NPCManagerScript tempObj = Instantiate(ObjectsToSpawn[objectindex], spawnLocations[objectSpawnPosIndex].position, Quaternion.identity);
        tempObj.Initialize();
        tempObj.transform.SetParent(enemiesParent);
        tempObj.gameObject.SetActive(false);
        spawnList.Add(tempObj);
    }

    

    IEnumerator SpawnEnemiesInProximity()
    {
        while (canSpawnEnemies)
        {
            Vector3 spawnLocation;
            int spawnLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Length);
            int objectToSpawnIndex = UnityEngine.Random.Range(0, ObjectsToSpawn.Length);

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

            currentSpawnInterval = spawnInterval + UnityEngine.Random.Range(-spawnIntervalOffset, spawnIntervalOffset);

            yield return new WaitForSeconds(currentSpawnInterval);

        }
    }

    internal void SpawnEnemiesInWaves(EnemyTypes type)
    {
        var spawnObj = spawnList.Find(new System.Predicate<NPCManagerScript>
            (c =>c.enemyType == type));
        int Idx = spawnList.FindIndex(new System.Predicate<NPCManagerScript>(t => t == spawnObj));

        Vector3 spawnLocation;
        spawnLocation = RandomNavSphere(spawnObj.transform.position, proxSpawnradius, -1);
        spawnObj.transform.position = spawnLocation;
        spawnObj.gameObject.SetActive(true);
        spawnList.Remove(spawnList[Idx]);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;


        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

        return navHit.position;
    }

    public int ObjectToSpawnIndex(EnemyTypes type)
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
}
