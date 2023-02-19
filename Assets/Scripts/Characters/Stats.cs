using DG.Tweening;
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


    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")]
    [SerializeField] private TextMeshProUGUI m_currentTowerTypeText;
    [SerializeField] private PlayerTower m_currentTower;

    [Space(2), Header("NPC EXCLUSIVE OPTIONS")]
    [SerializeField] private NPCManagerScript m_NPCManager;

    [Header("READONLY")]
    [ReadOnly] public bool ownerIsPlayer = false;


    bool isDead = false;
    private UIManager m_UIManager;
    private LevelManager m_LevelManager;
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

                if (ownerIsPlayer)
                {
                    if (TryGetComponent(out PlayerMainTower playerMainTower))
                    {
                        m_UIManager.GameOverVoid("You lost the level", false);
                    }
                }
                else
                {
                    m_LevelManager.deadEnemiesCount--;
                    m_UIManager.enemiesCountTxt.text = LevelManager.Instance.deadEnemiesCount.ToString();
                    if (m_LevelManager.deadEnemiesCount <= 0)
                    {
                        m_UIManager.GameOverVoid("You won the level", true);
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

        if (m_currentTower = GetComponent<PlayerTower>()) ownerIsPlayer = true;
        else m_NPCManager = GetComponent<NPCManagerScript>();

        if (ownerIsPlayer && m_currentTower) SetCurrentUnitTypeText(m_currentTower.TowerAttackType);

        m_currentHealth = Mathf.Clamp(Health, 0, m_MaxHealth);
    }
    private void FixedUpdate()
    {
        if (statsCanvas) statsCanvas.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    public void SetCurrentUnitTypeText(AttackType type)
    {
        if (m_currentTowerTypeText) m_currentTowerTypeText.text = m_UIManager.FormatStringNextLineOnUpperCase(type.ToString());
    }




    public void AddDamage(float damageAmount)
    {
        Health -= damageAmount;
        m_UIManager.ShowFloatingDamage(damageAmount, transform.position);
    }



    public void AddDamageOverTime(float duration, float damageAmount)
    {
        StartCoroutine(DamageOvertime(duration, damageAmount));
        m_UIManager.ShowFloatingDamage(damageAmount, transform.position);
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
    #region NPC EXCLUSIVES
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
    #endregion
}
