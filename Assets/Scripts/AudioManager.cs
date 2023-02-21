using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    internal AudioSource audioSource;

    public AudioClip LevelStart;
    public AudioClip SpellSound;
    public AudioClip BuildingConstruction;
    public AudioClip TowerUpgrade;
    public AudioClip TowerDestroyed;
    public AudioClip EnemyHit;
    public AudioClip Victory;
    public AudioClip LevelLost;
    public AudioClip EnemyDie;
    public AudioClip ManaFull;
    public AudioClip ManaOut;
    public AudioClip IceAttack;
    public AudioClip FireBallProjectile;
    public AudioClip FireBallHit;

    private void Awake()
    {
        if (Instance == null)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(gameObject);
        Instance = this;
        audioSource = this.GetComponent<AudioSource>();
    }
    
}
