using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTower : MonoBehaviour
{
    [SerializeField] private TurretController turret;
    [SerializeField] private Stats stats;

    private void Start()
    {
        if (!turret) turret = GetComponentInChildren<TurretController>();
    }

    private void Update()
    {
        turret.UpdateTurret();
    }


}
