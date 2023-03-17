using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletAOE : MonoBehaviour
{
    [SerializeField] protected float m_AOELifetime = 2f;
    [SerializeField] protected float m_AOEStartSize = 0;
    [SerializeField] protected float m_AOEEndSize = 1;
    [SerializeField] protected float m_AOETransitionTime = 2;
    [SerializeField] protected LayerMask groundLayer;

    [SerializeField] protected ParticleSystem m_ParticleEffect;

    protected Bullet m_OwnerBullet;

    public virtual void StartAOEEffect(Bullet OwnerBullet)
    {
        m_OwnerBullet = OwnerBullet;

        GetComponent<SphereCollider>().isTrigger = true;

        Ray ray = new(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point + new Vector3(0, 0.25f, 0);
        }

        this.gameObject.SetActive(true);

        if (m_ParticleEffect != null) m_ParticleEffect.Play();

        StartCoroutine(ExpandAOEOverTime());

    }

    protected virtual IEnumerator ExpandAOEOverTime()
    {
        float time = 0;

        while (time < m_AOETransitionTime)
        {
            float scale = Mathf.Lerp(m_AOEStartSize, m_AOEEndSize, time / m_AOETransitionTime);
            transform.localScale = new Vector3(scale, scale, scale);

            time += Time.deltaTime;
            yield return null;
        }

        // Make sure the scale is set to the final value
        transform.localScale = new Vector3(m_AOEEndSize, m_AOEEndSize, m_AOEEndSize);

        float timeLeftforAOE = Mathf.Clamp(m_AOELifetime, 0, m_AOELifetime - m_AOETransitionTime);

        yield return new WaitForSeconds(timeLeftforAOE);

        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NPCManagerScript hitNPC))
        {
            hitNPC._stats.damageNumberColor = m_OwnerBullet.associatedColor;
            hitNPC._stats.AddDamage(m_OwnerBullet.damage);
        }
    }
}
