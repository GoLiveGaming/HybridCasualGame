using UnityEngine;
using static MainPlayerControl;
public class Bullet : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float speed = 10f;
    public float lifetime = 2f;
    [SerializeField] private AttackType bulletAttackType;

    private Vector3 direction;
    private float elapsedTime = 0f;


    public void initializeBullet(Transform targetPos)
    {
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
            other.TryGetComponent(out NPCManagerScript npcManager);

            switch (bulletAttackType)
            {
                case AttackType.FireAttack:
                    StartWindAttack(npcManager);
                    break;

                case AttackType.WindAttack:
                    StartFireAttack(npcManager);
                    break;
            }
        }
    }

    private void StartWindAttack(NPCManagerScript hitNPC)
    {
        if (hitNPC._stats) hitNPC._stats.AddDamageOverTime(5, 2);


        //END ATTACK
        Destroy(gameObject);
    }
    private void StartFireAttack(NPCManagerScript hitNPC)
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Rigidbody rb);
            if (rb)
            {
                //Add Explosion Force
                rb.AddExplosionForce(20f, transform.position, 5, 1f, ForceMode.Impulse);
                //Modify Stats
                rb.GetComponent<NPCManagerScript>()._stats.AddDamage(5f);
            }
        }


        //END ATTACK
        Destroy(gameObject);
    }
}