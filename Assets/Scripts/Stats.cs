using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private float m_Health = 100;
    [SerializeField] private Image healthBar;

    [Header("ReadOnly Parameters"), Space(2)]
    public bool isDead = false;


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
