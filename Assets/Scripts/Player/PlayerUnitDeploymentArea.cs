using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField, ReadOnly] private PlayerTower deployedTower;

    private MainPlayerControl mainPlayerControl;
    private UIManager uiManager;

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        uiManager = UIManager.Instance;
        deployedTower = GetComponentInChildren<PlayerTower>();
    }
    public void OnUnitSelectionStarted()
    {
        MainPlayerControl.Instance.activeUnitDeploymentArea = this;
        for (int i = 0; i < unitSelectionCanvas.transform.childCount; i++)
        {
            unitSelectionCanvas.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    public void DeployAttackUnit(AttackType unitType)
    {
        PlayerTower unitSelectedToDeploy = mainPlayerControl.GetAttackUnitObject(unitType);

        if (!deployedTower)
        {
            DeployUnit(unitSelectedToDeploy);
        }
        else
        {
            CheckIfCanMerge(unitSelectedToDeploy);
        }

        mainPlayerControl.activeUnitDeploymentArea = null;
    }
    public void CheckIfCanMerge(PlayerTower towerSelectedToDeploy)
    {
        PlayerTower existingUnit = deployedTower;
        if (existingUnit.supportsCombining)
        {
            foreach (MergingCombinations existingUnitCombination in existingUnit.possibleCombinations)
            {
                if (towerSelectedToDeploy.TowerAttackType == existingUnitCombination.combinesWith)
                {
                    PlayerTower combinedTower = mainPlayerControl.GetAttackUnitObject(existingUnitCombination.toYield);
                    DeployUnit(combinedTower);
                    return;
                }
            }
        }
        else
        {
            DeployUnit(towerSelectedToDeploy);
            return;
        }
        Debug.Log("No Possible Combination Found");
    }
    public void DeployUnit(PlayerTower towerSelectedToDeploy)
    {
        if (towerSelectedToDeploy.resourceCost > mainPlayerControl.currentResourcesCount)
        {
            uiManager.ShowWarningText = towerSelectedToDeploy.TowerAttackType.ToString() + "Unit Needs: " + towerSelectedToDeploy.resourceCost.ToString() + "Gems";
            return;
        }
        DeleteChildTowers();

        PlayerTower spawnedTower = Instantiate(towerSelectedToDeploy, transform.position, Quaternion.identity);
        spawnedTower.transform.SetParent(this.transform, true);
        mainPlayerControl.RemoveResource(spawnedTower.resourceCost);
        deployedTower = spawnedTower;
    }
    private void DeleteChildTowers()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
