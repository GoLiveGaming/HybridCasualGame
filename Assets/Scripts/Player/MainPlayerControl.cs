using System.Collections.Generic;
using UnityEngine;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;
    [Header("Readonly Components")]
    [ReadOnly] public List<PlayerTower> activePlayerTowersList = new List<PlayerTower>();
    [ReadOnly] public PlayerUnitDeploymentArea activeUnitDeploymentArea;

    [Header("Player Components"), Space(2)]
    public Stats stats;

    [Header("Unit Deployment"), Space(2)]
    [SerializeField] private PlayerTower[] playerTowersPrefabs;

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
    private void Start()
    {
        stats = GetComponent<Stats>();
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
            if (tower.attackUnit.attackType == unitType)
                return tower.gameObject;
        }
        return null;
    }
}

public enum AttackType
{
    FireAttack,
    WindAttack,
    FireWindAttack
}