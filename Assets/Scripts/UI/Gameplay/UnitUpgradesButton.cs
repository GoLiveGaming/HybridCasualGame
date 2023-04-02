using UnityEngine;
using UnityEngine.UI;
public class UnitUpgradesButton : MonoBehaviour
{
    public Image combineWithIcon;
    public Image toYieldIcon;

    private PlayerTower ownerTower;
    AttackType toYieldUnitType;


    public void InitializeButton(PlayerUnit combinesWithUnit, PlayerUnit toYieldUnit, PlayerTower owner)
    {
        combineWithIcon.sprite = combinesWithUnit.statsUISprite;
        toYieldIcon.sprite = toYieldUnit.statsUISprite;
        ownerTower = owner;
        toYieldUnitType = toYieldUnit.unitType;

    }

    public void StartUpgrading()
    {
        ownerTower.deployedAtArea.UpgradeExistingAttackUnit(toYieldUnitType);
    }
}
