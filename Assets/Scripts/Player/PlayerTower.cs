using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTower : MonoBehaviour
{
    [SerializeField] private ShootingUnitController shootingUnitController;
    [SerializeField] private Stats stats;

    private void Start()
    {
        if (!shootingUnitController) shootingUnitController = GetComponentInChildren<ShootingUnitController>();
    }

    private void Update()
    {
        shootingUnitController.UpdateTurret();
    }


}
