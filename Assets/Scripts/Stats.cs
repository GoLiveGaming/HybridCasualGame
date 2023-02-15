using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_MaxHealth = 100;
    [SerializeField] private float m_currentHealth = 100;

    [Space(2), Header("STATS UI")]
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private Image m_healthBar;
    [ReadOnly] public bool ownerIsPlayer = false;


    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")]
    [SerializeField] private TextMeshProUGUI m_currentTowerTypeText;
    [SerializeField] private PlayerTower m_currentTower;

    [Space(2), Header("NPC EXCLUSIVE OPTIONS")]
    [SerializeField] private NPCManagerScript m_NPCManager;

    bool isDead = false;
    private UIManager m_UIManager;
    public float Health
    {
        get { return m_currentHealth; }
        set
        {
            m_currentHealth = value;
            m_currentHealth = Mathf.Clamp(Health, 0, m_MaxHealth);
            if (m_healthBar) m_healthBar.fillAmount = m_currentHealth / m_MaxHealth;
            if (m_currentHealth <= 0 && !isDead)
            {
                transform.TryGetComponent(out PlayerMainTower playerMainTower);
                if (playerMainTower) 
                {
                    UIManager.Instance.GameOverVoid("You lose the level", false);
                    Destroy(gameObject);
                    isDead = true;
                    return;
                }
                transform.TryGetComponent(out NPCManagerScript nPCManagerScript);
                if (nPCManagerScript)
                {
                    LevelManager.Instance.deadEnemiesCount--;
                    UIManager.Instance.enemiesCountTxt.text = LevelManager.Instance.deadEnemiesCount.ToString();
                    if (LevelManager.Instance.deadEnemiesCount <= 0)
                    {
                        UIManager.Instance.GameOverVoid("You won the level", true);  
                    }
                    Destroy(gameObject);
                    isDead = true;
                    return;
                }
                Destroy(gameObject);
                isDead = true;

            }
        }
    }
    public Image HealthBar
    {
        get { return m_healthBar; }
    }

    public void SetCurrentUnitTypeText(AttackType type)
    {
        if (m_currentTowerTypeText) m_currentTowerTypeText.text = m_UIManager.FormatStringNextLineOnUpperCase(type.ToString());
    }

    private void Start()
    {
        m_UIManager = UIManager.Instance;

        if (m_currentTower = GetComponent<PlayerTower>()) ownerIsPlayer = true;
        else m_NPCManager = GetComponent<NPCManagerScript>();

        if (ownerIsPlayer && m_currentTower) SetCurrentUnitTypeText(m_currentTower.TowerAttackType);

        m_currentHealth = Mathf.Clamp(Health, 0, m_MaxHealth);

    }
    private void FixedUpdate()
    {
        if (statsCanvas) statsCanvas.transform.rotation = Camera.main.transform.rotation;
    }



    /// <summary>
    /// Decrese Health
    /// </summary>
    /// <param name="damageAmount"></param>
    public void AddDamage(float damageAmount)
    {
        Health -= damageAmount;
    }

    /// <summary>
    /// Gradually decreases health over given amount of time
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="damageAmount"></param>
    public void AddDamageOverTime(float duration, float damageAmount)
    {
        StartCoroutine(DamageOvertime(duration, damageAmount));
    }

    private IEnumerator DamageOvertime(float damageDuration, float damagePerSecond)
    {
        while (damageDuration > 0)
        {
            Health -= damagePerSecond * Time.deltaTime;
            damageDuration -= Time.deltaTime;
            yield return null;
        }
    }

    public void SlowDownMoveSpeed(float speed, float duration)
    {
        if (!ownerIsPlayer && m_NPCManager)
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
}
