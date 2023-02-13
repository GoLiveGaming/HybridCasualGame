using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField, ReadOnly] private PlayerTower deployedTower;

    private MainPlayerControl mainPlayerControl;

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
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
                    DeleteChildTowers();
                    PlayerTower combinedTower = mainPlayerControl.GetAttackUnitObject(existingUnitCombination.toYield);
                    DeployUnit(combinedTower);
                    return;
                }
            }
        }
        else
        {
            DeleteChildTowers();
            DeployUnit(towerSelectedToDeploy);
            return;
        }
        Debug.Log("No Possible Combination Found");
    }
    public void DeployUnit(PlayerTower towerSelectedToDeploy)
    {
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
