using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradesButton : EnhancedButton
{
    public AttackType attackType;
    public Image buttonIcon;
    public TextMeshProUGUI costText;

    public PlayerUnitDeploymentArea unitDeploymentArea;

    public void InitializeButton(PlayerUnit playerUnit, PlayerUnitDeploymentArea playerUnitDeploymentArea)
    {
        attackType = playerUnit.unitType;
        unitDeploymentArea = playerUnitDeploymentArea;
        buttonIcon.sprite = playerUnit.unitPrefab.TowerIcon;
        costText.text = playerUnit.unitPrefab.resourceCost.ToString();
    }

    public void StartUpgrading()
    {
        unitDeploymentArea.UpgradeExistingAttackUnit(attackType);
    }
}
