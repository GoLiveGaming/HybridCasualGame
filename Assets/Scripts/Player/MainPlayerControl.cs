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

    [Header("Player Components"), Space(2)]
    public Stats stats;

    [Header("Unit Deployment"), Space(2)]
    [SerializeField] private PlayerUnit[] playerUnits;

    [Header("Units Controller"), Space(2)]
    public List<ShootingUnitController> shootingUnitControllers = new List<ShootingUnitController>();


    public enum AttackType
    {
        FireAttack,
        WindAttack,
        FireWindAttack
    }

    [System.Serializable]
    public class PlayerUnit
    {
        public AttackType unitType;
        public GameObject unitPrefab;
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        shootingUnitControllers = GetComponentsInChildren<ShootingUnitController>().ToList();
        stats = GetComponent<Stats>();
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

    public GameObject GetUnitToSpawn(AttackType unitType)
    {
        foreach (PlayerUnit unit in playerUnits)
        {
            if (unit.unitType == unitType)
                return unit.unitPrefab;
        }
        return null;
    }


}
