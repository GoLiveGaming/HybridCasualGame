using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Stats : MonoBehaviour
{
    [Space(2), Header("STATS UI")]
    [SerializeField] private float m_currentHealth = 100;
    public Color damageNumberColor = Color.white;

    [Space(5)] public bool isPlayer = true;

    public bool IsNpc
    {
        get { return !isPlayer; }
    }

    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")]
    [SerializeField, ShowIf("isPlayer")]
    private bool isMainTower = false;


    [Space(2), Header("NPC EXCLUSIVE OPTIONS")]
    [SerializeField, ShowIf("IsNpc")]
    private NPCManagerScript nPCManager;

    private Image healthbarImage;

    private Image HealthBarImage
    {
        get
        {
            if (healthbarImage == null)
            {
                if (isPlayer)
                {
                    if (isMainTower) healthbarImage = GetComponent<PlayerTowerMain>().playerTowerUI.HealthBarImage;
                    else healthbarImage = GetComponent<PlayerTower>().playerTowerUI.HealthBarImage;
                }
            }
            return healthbarImage;
        }
    }



    private bool _isDead = false;
    private UIManager _uiManager;
    private LevelManager _levelManager;
    private MainPlayerControl _mainPlayerControl;

    private float MaxHealth { get; set; }

    public float Health
    {
        get { return m_currentHealth; }
        private set
        {
            m_currentHealth = value;
            m_currentHealth = Mathf.Clamp(m_currentHealth, 0, MaxHealth);
            if (HealthBarImage) HealthBarImage.fillAmount = m_currentHealth / MaxHealth;

            CheckIfDead();
        }
    }


    private void CheckIfDead()
    {

        if ((m_currentHealth > 0) || _isDead) //Check if dead
        {
            return;
        }

        if (isPlayer)
        {
            if (isMainTower)
            {
                _mainPlayerControl.MainTowerHealthLostNum = (int)(MaxHealth - m_currentHealth);
                _uiManager.MatchFinished(false);
                if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.LevelLost);
            }
            else
            {
                _mainPlayerControl.TowersDestroyedNum++;
            }
        }
        else
        {
            _levelManager.deadEnemiesCount++;
            _mainPlayerControl.AddEnemiesKilledData(nPCManager.enemyType);
            if (_levelManager.AliveEnemiesLeft <= 0)
            {
                _uiManager.MatchFinished(true);
                if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.Victory);
            }
        }

        Destroy(gameObject);
        _isDead = true;
    }

    public void InitializeStats()
    {
        _uiManager = UIManager.Instance;
        _levelManager = LevelManager.Instance;
        _mainPlayerControl = MainPlayerControl.Instance;

        if (isPlayer)
        {
            if (isMainTower) healthbarImage = GetComponent<PlayerTowerMain>().playerTowerUI.HealthBarImage;
            else healthbarImage = GetComponent<PlayerTower>().playerTowerUI.HealthBarImage;
        }
        else nPCManager = GetComponent<NPCManagerScript>();

        MaxHealth = m_currentHealth;

    }

    public void AddDamage(float damageAmount)
    {
        Health -= damageAmount;
        if (!isPlayer)
            _uiManager.ShowFloatingScore(damageAmount, transform.position, damageNumberColor);
    }


    public void AddDamageOverTime(float duration, float damageAmount)
    {
        StartCoroutine(DamageOvertime(duration, damageAmount));
        if (!isPlayer)
            _uiManager.ShowFloatingScore(damageAmount, transform.position, damageNumberColor);
    }

    private IEnumerator DamageOvertime(float damageDuration, float damagePerSecond)
    {
        Material[] meshMaterials = GetComponentInChildren<SkinnedMeshRenderer>().materials;

        foreach (Material mat in meshMaterials)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", damageNumberColor);
        }

        while (damageDuration > 0)
        {
            Health -= damagePerSecond * Time.deltaTime;
            damageDuration -= Time.deltaTime;
            yield return null;
        }

        foreach (Material mat in meshMaterials)
        {
            mat.DisableKeyword("_EMISSION");
        }
    }

    #region NPC EXCLUSIVES

    public void SlowDownMoveSpeed(float speed, float duration)
    {
        if (!isPlayer && nPCManager)
        {
            StartCoroutine(StartSlowMoveSpeed(speed, duration));
        }
    }

    private IEnumerator StartSlowMoveSpeed(float speed, float duration)
    {
        nPCManager.SetMoveSpeed(speed);
        yield return new WaitForSeconds(duration);
        nPCManager.ResetMoveSpeed();
    }

    #endregion
}