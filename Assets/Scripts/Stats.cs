using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_currentHealth = 100;
    [SerializeField] private float m_MaxHealth = 100;
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private Image m_healthBar;
    [SerializeField] private TextMeshProUGUI m_currentLeveltext;


    [Header("ReadOnly Parameters"), Space(2)]
    [ReadOnly] public bool isDead = false;
    public float Health
    {
        get { return m_currentHealth; }
        set
        {
            m_currentHealth = value;
            m_currentHealth = Mathf.Clamp(Health, 0, m_MaxHealth);
            if (m_healthBar) m_healthBar.fillAmount = m_currentHealth / m_MaxHealth;
            if (Health <= 0)
            {
                transform.TryGetComponent(out PlayerTower playerTower);
                if (playerTower) UIManager.Instance.ShowText("Game Over");
                Destroy(gameObject);
                isDead = true;
            }
        }
    }
    public Image HealthBar
    {
        get { return m_healthBar; }
    }

    public void SetCurrentLeveltext(int lvl)
    {
        if (m_currentLeveltext) m_currentLeveltext.text = lvl.ToString();
    }

    private void Awake()
    {
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
}
