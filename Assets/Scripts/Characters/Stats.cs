using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_currentHealth = 100;
    public Color damageNumberColor = Color.white;

    [Space(2), Header("STATS UI")]
    [SerializeField] private GameObject statsCanvas;
    public Image m_healthBar;

    [Space(5)]
    public bool isPlayer = true;
    public bool IsNPC { get { return !isPlayer; } }

    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")]
    [SerializeField, ShowIf("isPlayer")] private TextMeshProUGUI m_currentTowerTypeText;
    [SerializeField, ShowIf("isPlayer")] private PlayerUnitBase m_currentTower;


    [Space(5)]
    [Space(2), Header("NPC EXCLUSIVE OPTIONS")]
    [SerializeField, ShowIf("IsNPC")] private NPCManagerScript m_NPCManager;
    [SerializeField] private float killedScore = 1;

    bool isDead = false;
    private UIManager m_UIManager;
    private LevelManager m_LevelManager;
    private MainPlayerControl m_MainPlayerControl;

    public float KilledScore { get { return killedScore; } }
    public float MaxHealth { get; private set; }
    public float Health
    {
        get { return m_currentHealth; }
        set
        {
            m_currentHealth = value;
            m_currentHealth = Mathf.Clamp(Health, 0, MaxHealth);
            if (m_healthBar) m_healthBar.fillAmount = m_currentHealth / MaxHealth;

            if (m_currentHealth <= 0 && !isDead)
            {

                if (isPlayer)
                {
                    if (GetComponent<PlayerMainTower>())
                    {
                        m_UIManager.MatchFinished(false);
                        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.LevelLost);
                    }
                    if (m_currentTower.GetComponent<PlayerTower>())
                    {
                        m_MainPlayerControl.TowersDestroyedNum++;
                    }
                }
                else
                {
                    m_LevelManager.deadEnemiesCount++;
                    m_MainPlayerControl.AddEnemiesKilledData(m_NPCManager.enemyType);
                    if (m_LevelManager.AliveEnemiesLeft <= 0)
                    {
                        m_UIManager.MatchFinished(true);
                        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.Victory);
                    }
                }
                Destroy(gameObject);
                isDead = true;

            }
        }
    }
    private void Start()
    {
        m_UIManager = UIManager.Instance;
        m_LevelManager = LevelManager.Instance;
        m_MainPlayerControl = MainPlayerControl.Instance;

        if (m_currentTower = GetComponent<PlayerUnitBase>()) isPlayer = true;
        else m_NPCManager = GetComponent<NPCManagerScript>();

        MaxHealth = m_currentHealth;

        if (statsCanvas) statsCanvas.transform.rotation = Camera.main.transform.rotation;
    }


    public void AddDamage(float damageAmount)
    {
        Health -= damageAmount;
        if (!isPlayer)
            m_UIManager.ShowFloatingScore(damageAmount, transform.position, damageNumberColor);
    }


    public void AddDamageOverTime(float duration, float damageAmount)
    {
        StartCoroutine(DamageOvertime(duration, damageAmount));
        if (!isPlayer)
            m_UIManager.ShowFloatingScore(damageAmount, transform.position, damageNumberColor);
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
        m_UIManager.ShowFloatingResourceRemovedUI(value, transform.position, Color.white);
    }
    #endregion

    #region NPC EXCLUSIVES
    public void SlowDownMoveSpeed(float speed, float duration)
    {
        if (!isPlayer && m_NPCManager)
        {
            StartCoroutine(StartSlowMoveSpeed(speed, duration));
        }
    }

    private IEnumerator StartSlowMoveSpeed(float speed, float duration)
    {
        m_NPCManager.SetMoveSpeed(speed);
        yield return new WaitForSeconds(duration);
        m_NPCManager.ResetMoveSpeed();

    }
    #endregion
}
