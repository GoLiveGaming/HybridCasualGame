using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField, ReadOnly] private PlayerTower parentTower;

    private MainPlayerControl mainPlayerControl;

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        parentTower = GetComponentInParent<PlayerTower>();
    }
    public void OnUnitSelectionStarted()
    {
        MainPlayerControl.Instance.activeUnitDeploymentArea = this;
        unitSelectionCanvas.SetActive(true);
    }

    public void DeployAttackUnit(AttackType unitType)
    {

        AttackUnit unitSelectedToDeploy = mainPlayerControl.GetAttackUnitObject(unitType);

        if (!parentTower.attackUnit)
        {
            DeployNewUnit(unitSelectedToDeploy);
        }
        else
        {
            MergeWithNewUnit(unitSelectedToDeploy);
        }
    }

    public void DeployNewUnit(AttackUnit unitSelectedToDeploy)
    {
        DeleteChildAttackUnits();

        AttackUnit spawnedAttackUnit = Instantiate(unitSelectedToDeploy, transform.position, Quaternion.identity);
        spawnedAttackUnit.transform.SetParent(this.transform, true);
        parentTower.ReInitializeTower(spawnedAttackUnit);
    }

    public void MergeWithNewUnit(AttackUnit unitSelectedToDeploy)
    {
        AttackUnit existingUnit = parentTower.attackUnit;
        if (existingUnit.supportsCombining)
        {
            foreach (MergingCombinations existingUnitCombination in existingUnit.possibleCombinations)
            {
                if (unitSelectedToDeploy.attackType == existingUnitCombination.combinesWith)
                {
                    DeleteChildAttackUnits();
                    AttackUnit combinedUnit = mainPlayerControl.GetAttackUnitObject(existingUnitCombination.toYield);
                    DeployNewUnit(combinedUnit);
                    return;
                }
            }
        }
        else
        {
            DeleteChildAttackUnits();
            DeployNewUnit(unitSelectedToDeploy);
            return;
        }
        Debug.Log("No Possible Combination Found");
    }

    private void DeleteChildAttackUnits()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
