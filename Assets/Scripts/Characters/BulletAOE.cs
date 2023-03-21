using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletAOE : MonoBehaviour
{
    [SerializeField] protected float m_AOELifetime = 2f;
    [SerializeField] protected float m_AOEStartSize = 0;
    [SerializeField] protected float m_AOESizeIncrement = 1;
    [SerializeField] protected float m_AOETransitionTime = 2;
    [SerializeField] protected LayerMask groundLayer;

    [SerializeField] protected ParticleSystem m_ParticleEffect;



    public virtual void StartAOEEffect()
    {

        GetComponent<SphereCollider>().isTrigger = true;

        Ray ray = new(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point + new Vector3(0, 0.25f, 0);
        }

        this.gameObject.SetActive(true);

        if (m_ParticleEffect != null) m_ParticleEffect.Play();

        StartExpandingAOE();

    }

    private float m_AccumulatedDesiredSize = 0;
    private float m_ElapsedTime = 0;
    private bool m_IsScaling = false;

    public void StartExpandingAOE()
    {
        // Add the desired size to the accumulated desired size
        m_AccumulatedDesiredSize += m_AOESizeIncrement;

        if (!m_IsScaling)
        {
            // If the object is not currently scaling, start the coroutine
            m_IsScaling = true;
            StartCoroutine(ScaleOverTime());
        }
    }

    protected virtual IEnumerator ScaleOverTime()
    {
        // Set the start scale and final scale based on the accumulated desired size
        float startScale = transform.localScale.x;
        float finalScale = startScale + m_AccumulatedDesiredSize;

        // Set the elapsed time to zero
        m_ElapsedTime = 0;

        while (m_ElapsedTime < m_AOETransitionTime)
        {
            // Calculate the scale for this frame
            float scale = Mathf.Lerp(startScale, finalScale, m_ElapsedTime / m_AOETransitionTime);

            // Set the scale of the object
            transform.localScale = new Vector3(scale, scale, scale);

            // Increment the elapsed time
            m_ElapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Set the final scale of the object
        transform.localScale = new Vector3(finalScale, finalScale, finalScale);

        // Reset the accumulated desired size and elapsed time
        m_AccumulatedDesiredSize = 0;
        m_ElapsedTime = 0;

        // Set the scaling flag to false
        m_IsScaling = false;

        // Destroy the object after the specified lifetime
        yield return new WaitForSeconds(m_AOELifetime - m_AOETransitionTime);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
    }
}
