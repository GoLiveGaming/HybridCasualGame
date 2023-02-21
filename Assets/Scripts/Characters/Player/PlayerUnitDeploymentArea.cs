using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [ReadOnly] public PlayerTower deployedTower;
    [ReadOnly] public bool isAreaAvailable = true;
    private MainPlayerControl mainPlayerControl;
    private UIManager uiManager;
    private BoxCollider boxCollider;

    private void OnDrawGizmos()
    {
        // Draw a semitransparent red cube at the transforms position
        if (!boxCollider)
            boxCollider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1, 0, 0.25f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z));
    }
    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        uiManager = UIManager.Instance;
        deployedTower = GetComponentInChildren<PlayerTower>();
        isAreaAvailable = true;
    }

    public void DeployAttackUnit(AttackType unitType)
    {
        if (!isAreaAvailable) { Debug.Log("Area Not Available"); return; }

        PlayerTower unitSelectedToDeploy = mainPlayerControl.GetAttackUnitObject(unitType);
        StartDeployemnt(unitSelectedToDeploy);

    }
    public void StartDeployemnt(PlayerTower towerSelectedToDeploy)
    {
        PlayerTower existingUnit = deployedTower;

        if (existingUnit == null)
        {
            DeployUnit(towerSelectedToDeploy);
            if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.BuildingConstruction);
        }
        else
        {
            DeployUnit(GetUnitAfterMergeCheck(towerSelectedToDeploy));
            if (AudioManager.Instance) AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.TowerUpgrade);
            SpawnParticles(1, -90);
        }

    }

    void SpawnParticles(int particleIndex, int rotation)
    {
        ParticleSystem particleTemp = Instantiate(MainPlayerControl.Instance.towerParticles[particleIndex], transform.position, Quaternion.Euler(rotation, 0f, 0f));
        Destroy(particleTemp.gameObject, particleTemp.main.duration);
    }
    public PlayerTower GetUnitAfterMergeCheck(PlayerTower towerSelectedToDeploy)
    {
        PlayerTower existingUnit = deployedTower;
        if (existingUnit == null)
        {
            return towerSelectedToDeploy;
        }
        if (existingUnit && existingUnit.supportsCombining)
        {
            foreach (MergingCombinations existingUnitCombination in existingUnit.possibleCombinations)
            {
                if (towerSelectedToDeploy.TowerAttackType == existingUnitCombination.combinesWith)
                {
                    PlayerTower combinedTower = mainPlayerControl.GetAttackUnitObject(existingUnitCombination.toYield);
                    return combinedTower;
                }
            }
        }

        Debug.Log("No possible merge combinations found for: " + towerSelectedToDeploy);
        return towerSelectedToDeploy;


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


    public void ToggleHighlightArea(bool value)
    {
        TryGetComponent(out MeshRenderer renderer);
        if (renderer)
        {
            if (value == true)
            {
                if (isAreaAvailable)
                {
                    renderer.material.color = Color.green;
                }
                else
                {
                    renderer.material.color = Color.red;
                }
            }
            else
            {
                renderer.material.color = new Color(1, 1, 1, 0);
            }
        }
    }

}
