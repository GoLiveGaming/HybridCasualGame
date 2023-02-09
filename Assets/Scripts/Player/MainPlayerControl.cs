using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;
    [Header("Readonly Components")]
    [ReadOnly] public List<PlayerTower> activePlayerTowersList = new List<PlayerTower>();
    [ReadOnly] public PlayerUnitDeploymentArea activeUnitDeploymentArea;

    [Header("Units Propertie"), Space(2)]
    public PlayerTower[] playerTowersPrefabs;
    public Bullets[] attackBulletVariants;

    [System.Serializable]
    public class PlayerUnit
    {
        public AttackType unitType;
        public GameObject unitPrefab;
    }

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        if (Input.GetButtonDown("Fire1")) SelectUnitDeploymentArea();
    }

    void SelectUnitDeploymentArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.TryGetComponent(out PlayerUnitDeploymentArea playerUnitDeploymentArea))
            {
                playerUnitDeploymentArea.OnUnitSelected();
            }
        }
    }

    public GameObject GetUnitToSpawn(AttackType unitType)
    {
        foreach (PlayerTower tower in playerTowersPrefabs)
        {
            if (tower.attackUnit.attackUnitType == unitType)
                return tower.gameObject;
        }
        return null;
    }
}

public enum AttackType
{
    FireAttack,
    WindAttack,
    LightningAttack
}

[Serializable]
public class Bullets
{
    public GameObject bulletPrefab;
    public AttackType associatedAttack;
}