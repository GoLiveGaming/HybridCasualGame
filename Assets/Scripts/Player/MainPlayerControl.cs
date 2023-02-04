using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl instance;

    [Header("Readonly Components")]
    public PlayerUnitDeploymentArea activeUnitDeploymentArea;

    [Header("Unit Deployment"), Space(2)]
    [SerializeField] private PlayerUnit[] playerUnits;

    [Header("Units Controller"), Space(2)]
    public List<ShootingUnitController> shootingUnitControllers = new List<ShootingUnitController>();
    [SerializeField] private Stats stats;

    public enum PlayerUnitType
    {
        FireAttackUnit,
        WindAttackUnit,
        FireWindAttackUnit
    }

    [System.Serializable]
    public class PlayerUnit
    {
        public PlayerUnitType unitType;
        public GameObject unitPrefab;
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        shootingUnitControllers = GetComponentsInChildren<ShootingUnitController>().ToList();
    }

    void Update()
    {
        foreach (ShootingUnitController controller in shootingUnitControllers)
        {
            controller.UpdateTurret();
        }
        GetInputs();
    }

    private void GetInputs()
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

    public GameObject GetUnitToSpawn(PlayerUnitType unitType)
    {
        foreach (PlayerUnit unit in playerUnits)
        {
            if (unit.unitType == unitType)
                return unit.unitPrefab;
        }
        return null;
    }


}
