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

        //if (Vector3.Distance(transform.position, target.position) < 0.1f)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TurretTarget")
        {
            switch (parentUnitType)
            {
                case PlayerUnitType.FireAttackUnit:
                    other.GetComponent<TargetEnemy>().stats.Damage += 20f;
                   // other.GetComponent<Rigidbody>().AddForce(transform.forward * 150f);
                    Destroy(gameObject);
                    break;
                case PlayerUnitType.WindAttackUnit:
                    
                    //  other.GetComponent<Rigidbody>().AddForce(transform.forward * 150f);

                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
                    foreach (var hitCollider in hitColliders)
                    {
                        hitCollider.TryGetComponent(out Rigidbody rb);
                        if (rb) 
                        {
                            rb.AddExplosionForce(5f, transform.position, 200, 0f, ForceMode.Impulse);
                            rb.GetComponent<TargetEnemy>().stats.Damage += 5f;
                        }
                            
                        //if (hitCollider.CompareTag("TurretTarget"))
                        //{
                        //}
                    }
                    
                    Destroy(gameObject);
                    break;
            }
            
        }
    }
}