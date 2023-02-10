using UnityEngine;

[RequireComponent(typeof(Stats))]
public class PlayerTower : MonoBehaviour
{
    [Space(2), Header("READONLY PARAMETERS")]
    [ReadOnly] public bool attackUnitAvailable = false;

    [Space(2), Header("READONLY COMPONENTS")]
    [ReadOnly] public MainPlayerControl mainPlayerControl;
    [ReadOnly] public AttackUnit attackUnit;
    [ReadOnly] public Stats stats;

    private void OnEnable()
    {
        if (mainPlayerControl)
            mainPlayerControl.activePlayerTowersList.Add(this);
    }
    private void OnDisable()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        mainPlayerControl.activePlayerTowersList.Remove(this);
    }
    private void Awake()
    {
        stats = GetComponent<Stats>();

        attackUnitAvailable = false;
    }
    private void Start()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        mainPlayerControl.activePlayerTowersList.Add(this);
    }

    private void Update()
    {
        UpdateTower();
    }
    public void UpdateTower()
    {
        if (attackUnit) attackUnit.UpdateUnit();
    }
    [ContextMenu("Reinitialize")]
    public void ReInitializeTower(AttackUnit unit)
    {
        attackUnit = unit;
        attackUnitAvailable = true;
    }

}
