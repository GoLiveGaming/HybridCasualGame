using UnityEngine;
public class Bullet : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    public float damage = 2f;
    public LayerMask collisionLayerMask;

    private Vector3 direction;
    private float elapsedTime = 0f;


    public void initializeBullet(Transform targetTF)
    {

        target = targetTF;
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        direction = (target.position - transform.position).normalized;
    }
    void Update()
    {


        transform.position = transform.position + direction * speed * Time.deltaTime;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }

        
    }
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
   // protected virtual void OnTriggerEnter(Collider other) { }


    protected virtual void StartAttack(NPCManagerScript hitNPC) { }

    protected virtual void StartAttackTower(PlayerTower hitMain) { }
}