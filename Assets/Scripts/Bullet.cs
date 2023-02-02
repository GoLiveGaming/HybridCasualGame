using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float speed = 10f;
    public float lifetime = 2f;

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

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}