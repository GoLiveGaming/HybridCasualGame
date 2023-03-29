using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_currentHealth = 100;
    public Color damageNumberColor = Color.white;

    [Space(2), Header("STATS UI")] [SerializeField]
    private GameObject statsCanvas;

    public Image m_healthBar;

    [Space(5)] public bool isPlayer = true;

    public bool IsNpc
    {
        get { return !isPlayer; }
    }

    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")] [SerializeField, ShowIf("isPlayer")]
    private PlayerUnitBase currentTower;

    [SerializeField, ShowIf("isPlayer")] private bool isMainTower = false;


    [Space(5)] [Space(2), Header("NPC EXCLUSIVE OPTIONS")] [SerializeField, ShowIf("IsNPC")]
    private NPCManagerScript nPCManager;

    [SerializeField] private float killedScore = 1;

    private bool _isDead = false;
    private UIManager _uiManager;
    private LevelManager _levelManager;
    private MainPlayerControl _mainPlayerControl;

    public float KilledScore
    {
        get { return killedScore; }
    }

    private float MaxHealth { get; set; }

    public float Health
    {
        get { return m_currentHealth; }
        private set
        {
            m_currentHealth = value;
            m_currentHealth = Mathf.Clamp(Health, 0, MaxHealth);
            if (m_healthBar) m_healthBar.fillAmount = m_currentHealth / MaxHealth;

            if (!(m_currentHealth <= 0) || _isDead)
            {
                return;
            }

            if (isPlayer)
            {
                if (isMainTower)
                {
                    _uiManager.MatchFinished(false);
                    _mainPlayerControl.MainTowerHealthLostNum = (int)(MaxHealth - m_currentHealth);
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
    }

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _levelManager = LevelManager.Instance;
        _mainPlayerControl = MainPlayerControl.Instance;

        currentTower = GetComponent<PlayerUnitBase>();
        if (currentTower)
        {
            if (GetComponent<PlayerMainTower>()) isMainTower = true;
            isPlayer = true;
        }
        else nPCManager = GetComponent<NPCManagerScript>();

        MaxHealth = m_currentHealth;

        if (statsCanvas) statsCanvas.transform.rotation = Camera.main.transform.rotation;
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

    #region PLAYER EXCLUSIVES

    public void ShowResourceRemovedUI(string value)
    {
        _uiManager.ShowFloatingResourceRemovedUI(value, transform.position, Color.white);
    }

    #endregion

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