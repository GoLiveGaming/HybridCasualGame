using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainPlayerControl;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float speed = 10f;
    public float lifetime = 2f;

    private Vector3 direction;
    private float elapsedTime = 0f;
    PlayerUnitType parentUnitType;

    public void initializeBullet(Transform targetPos, PlayerUnitType playerUnitType)
    {
        parentUnitType = playerUnitType;
        target = targetPos;
        direction = (target.position - transform.position).normalized;
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = transform.position + direction * speed * Time.deltaTime;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetEnemy"))
        {
            other.TryGetComponent(out TargetEnemy targetEnemy);
            other.TryGetComponent(out Stats stats);
            other.TryGetComponent(out NPCStateManager npcSM);

            switch (parentUnitType)
            {
                case PlayerUnitType.FireAttackUnit:

                    if (stats) stats.StartDamageOverTime();
                    Destroy(gameObject);
                    break;

                case PlayerUnitType.WindAttackUnit:
                    StartWindAttackEffect();
                    Destroy(gameObject);
                    break;
            }

        }
    }

    void StartWindAttackEffect()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                rb.AddExplosionForce(5f, transform.position, 5, 1f, ForceMode.Impulse);

                //Modify Stats
                rb.GetComponent<Stats>().AddDamage(5f);
            }
        }
    }
}