using UnityEngine;
public class Bullet : MonoBehaviour
{
    [HideInInspector] public Vector3 targetPos;
    public float speed = 10f;
    public float lifetime = 2f;

    public float damage = 2f;
    public LayerMask collisionLayerMask;

    private Vector3 direction;
    private float elapsedTime = 0f;


    public virtual void InitializeBullet(Vector3 target)
    {

        targetPos = target;
        if (this.targetPos == null)
        {
            Destroy(gameObject);
            return;
        }
        direction = (targetPos - transform.position).normalized;
    }
    protected virtual void Update()
    {
        transform.position = transform.position + (direction * speed * Time.deltaTime);

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }


    }
    protected static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void StartAttack(NPCManagerScript hitNPC) { }

    protected virtual void StartAttackTower(PlayerTower hitMain) { }
}
