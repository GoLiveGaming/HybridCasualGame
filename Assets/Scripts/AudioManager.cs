using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{
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

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

}
