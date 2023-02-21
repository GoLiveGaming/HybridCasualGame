using UnityEngine;

[RequireComponent(typeof(Stats))]
public class PlayerUnitBase : MonoBehaviour
{
    public AttackType TowerAttackType;
    [ReadOnly] public MainPlayerControl mainPlayerControl;
    protected void AddUnitToMain()
    {

        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        if (mainPlayerControl && !mainPlayerControl.activePlayerTowersList.Contains(this))
            mainPlayerControl.activePlayerTowersList.Add(this);

    }

    protected void RemoveUnitFromMain()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        if (mainPlayerControl && mainPlayerControl.activePlayerTowersList.Contains(this))
            mainPlayerControl.activePlayerTowersList.Remove(this);
    }
}
