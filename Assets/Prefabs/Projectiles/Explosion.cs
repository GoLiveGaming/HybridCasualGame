using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("TargetEnemy"))
        {
            
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject,5f);
        }
        
    }
}
