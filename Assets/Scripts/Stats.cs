using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_Health = 100;

    [Header("ReadOnly Parameters"), Space(2)]
    [SerializeField] private bool isStunned = false;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] NPCStateManager m_NPCStateManager;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NPCStateManager = GetComponent<NPCStateManager>();
    }

    public float Health
    {
        get { return m_Health; }
        set
        {
            m_Health = value;
            if (healthBar) healthBar.fillAmount = m_Health / 100;
            if (Health <= 0) { Destroy(gameObject); isDead = true; }
        }
    }
    public bool isDead = false;
    public Image healthBar;

    public void StartDamageOverTime()
    {
        StartCoroutine(DamageOvertime(5, 2));
    }
    public void AddDamage(float damage)
    {
        Health -= damage;
    }


    private IEnumerator DamageOvertime(float damageDuration, float damagePerSecond)
    {
        while (damageDuration > 0)
        {
            Health -= damagePerSecond * Time.deltaTime;
            damageDuration -= Time.deltaTime;
            yield return null;

            Debug.Log(damagePerSecond);
        }
    }
}
