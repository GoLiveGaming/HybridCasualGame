using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(BoxCollider))]
public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [ReadOnly] public PlayerTower deployedTower;
    private MainPlayerControl _mainPlayerControl;
    private UIManager _uiManager;
    private PlayerDataManager _dataManager;
    private AudioManager _audioManager;

    private BoxCollider boxCollider;

    public bool HasDeployedUnit { get { return deployedTower; } }

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
        _mainPlayerControl = MainPlayerControl.Instance;
        _uiManager = UIManager.Instance;
        _audioManager = AudioManager.Instance;
        _dataManager = PlayerDataManager.Instance;

    }

    public void DeployAttackUnit(AttackType unitType)
    {
        if (HasDeployedUnit) { Debug.Log("Area Not Available"); return; }

        PlayerUnit unitSelectedToDeploy = _mainPlayerControl.GetPlayerUnit(unitType);
        DeployUnit(unitSelectedToDeploy);
        _mainPlayerControl.TowersPlacedNum++;
    }

    public virtual void UpgradeExistingAttackUnit(AttackType unitType)
    {
        PlayerUnit unitSelectedToDeploy = _mainPlayerControl.GetPlayerUnit(unitType);
        DeployUnit(GetUnitAfterMergeCheck(unitSelectedToDeploy));
        _mainPlayerControl.TowersUpgradedNum++;
    }

    public PlayerUnit GetUnitAfterMergeCheck(PlayerUnit towerSelectedToDeploy)
    {
        PlayerTower existingUnit = deployedTower;

        if (existingUnit && existingUnit.supportsCombining)
        {
            foreach (MergingCombinations existingUnitCombination in existingUnit.possibleCombinations)
            {
                if (towerSelectedToDeploy.unitPrefab.TowerAttackType == existingUnitCombination.toYield)
                {
                    PlayerUnit combinedTower = _mainPlayerControl.GetPlayerUnit(existingUnitCombination.toYield);

                    if (!_dataManager.IsAttackTypeUnlocked(combinedTower.unitType)) break;
                    Debug.Log("Upgrading to: " + combinedTower);
                    return combinedTower;
                }
            }
        }
        Debug.Log("No possible merge combinations found for: " + towerSelectedToDeploy.unitType);
        return null;
    }

    public void DeployUnit(PlayerUnit towerSelectedToDeploy)
    {
        if (towerSelectedToDeploy == null)
        {
            _uiManager.ShowWarningText = "Selected Unit Cannot be placed here.";
            return;
        }
        if (towerSelectedToDeploy.unitPrefab.resourceCost > _mainPlayerControl.currentResourcesCount)
        {
            _uiManager.ShowWarningText = towerSelectedToDeploy.unitPrefab.TowerAttackType.ToString() + "Unit Needs: " + towerSelectedToDeploy.unitPrefab.resourceCost.ToString() + "Gems";
            return;
        }
        DeleteChildTowers();

        PlayerTower spawnedTower = Instantiate(towerSelectedToDeploy.unitPrefab, transform.position, Quaternion.identity);
        spawnedTower.transform.SetParent(this.transform, true);
        _mainPlayerControl.RemoveResource(spawnedTower.resourceCost);
        if (_audioManager)
        {
            if (deployedTower != null)
                _audioManager.audioSource.PlayOneShot(AudioManager.Instance.BuildingConstruction);
            else
                _audioManager.audioSource.PlayOneShot(AudioManager.Instance.TowerUpgrade);

        }
        deployedTower = spawnedTower;
        _uiManager.unitUpgradesPanel.SetActive(false);

        SpawnParticles(1, -90);
    }

    void SpawnParticles(int particleIndex, int rotation)
    {
        ParticleSystem particleTemp = Instantiate(MainPlayerControl.Instance.towerParticles[particleIndex], transform.position, Quaternion.Euler(rotation, 0f, 0f));
        Destroy(particleTemp.gameObject, particleTemp.main.duration);
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
                if (!HasDeployedUnit)
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
                renderer.material.color = Color.clear;
            }
        }
    }

}
