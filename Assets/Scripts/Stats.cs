using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public bool isDead = false;
    public float Health = 100;
    public float Damage;
    public Image healtBar;

    private void FixedUpdate()
    {
        healtBar.fillAmount = Health / 100f;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
