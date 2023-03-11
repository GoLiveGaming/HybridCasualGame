using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_MaxHealth = 100;
    [SerializeField] private float m_currentHealth = 100;
    public Color damageNumberColor = Color.white;

    [Space(2), Header("STATS UI")]
    [SerializeField] private GameObject statsCanvas;
    public Image m_healthBar;
    

    [Space(2), Header("PLAYER EXCLUSIVE OPTIONS")]
    [SerializeField] private TextMeshProUGUI m_currentTowerTypeText;
    [SerializeField] private PlayerUnitBase m_currentTower;

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
                    if (GetComponent<PlayerMainTower>())
                    {
                        m_UIManager.MatchFinished(false);
                        if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.LevelLost);
                    }
                    if (m_currentTower.TryGetComponent(out PlayerTower tower))
                        tower.OnTowerDestroyed();
                }
                else
                {
                    m_LevelManager.deadEnemiesCount--;
                    m_UIManager.enemiesCountTxt.text = LevelManager.Instance.deadEnemiesCount.ToString();
                    if (m_LevelManager.deadEnemiesCount <= 0)
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

        if (m_currentTower = GetComponent<PlayerUnitBase>()) ownerIsPlayer = true;
        else m_NPCManager = GetComponent<NPCManagerScript>();

        if (ownerIsPlayer && m_currentTower) SetCurrentUnitTypeText(m_currentTower.TowerAttackType);
        if (ownerIsPlayer && m_currentTower) SetCurrentUnitTypeText(m_currentTower.TowerAttackType);

        m_currentHealth = Mathf.Clamp(Health, 0, m_MaxHealth);

        if (statsCanvas) statsCanvas.transform.rotation = Camera.main.transform.rotation;
    }
    public void SetCurrentUnitTypeText(AttackType type)
    {
        if (m_currentTowerTypeText) m_currentTowerTypeText.text = m_UIManager.FormatStringNextLineOnUpperCase(type.ToString());
    }




    public void AddDamage(float damageAmount)
    {
        Health -= damageAmount;
        m_UIManager.ShowFloatingDamage(damageAmount, transform.position, damageNumberColor);
    }



    public void AddDamageOverTime(float duration, float damageAmount)
    {
        StartCoroutine(DamageOvertime(duration, damageAmount));
        m_UIManager.ShowFloatingDamage(damageAmount, transform.position, damageNumberColor);
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
    #region PLAYER EXCLUSIVES
    public void ShowFloatingText(string value)
    {
        m_UIManager.ShowFloatingText(value, transform.position, Color.white);
    }
    #endregion

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
