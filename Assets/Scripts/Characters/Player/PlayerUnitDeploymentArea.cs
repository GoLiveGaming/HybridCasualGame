using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [ReadOnly] public PlayerTower deployedTower;
    [ReadOnly] public bool isAreaAvailable = true;
    private MainPlayerControl _mainPlayerControl;
    private UIManager _uiManager;
    private PlayerDataManager _dataManager;
    private AudioManager _audioManager;

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
        _mainPlayerControl = MainPlayerControl.Instance;
        _uiManager = UIManager.Instance;
        _audioManager = AudioManager.Instance;
        _dataManager = PlayerDataManager.Instance;


        deployedTower = GetComponentInChildren<PlayerTower>();
        isAreaAvailable = true;
    }

    public void DeployAttackUnit(AttackType unitType)
    {
        //if (!isAreaAvailable) { Debug.Log("Area Not Available"); return; }

        PlayerUnit unitSelectedToDeploy = _mainPlayerControl.GetPlayerUnit(unitType);
        StartDeployemnt(unitSelectedToDeploy);

    }
    public void StartDeployemnt(PlayerUnit towerSelectedToDeploy)
    {
        PlayerTower existingUnit = deployedTower;

        if (existingUnit == null)
        {
            DeployUnit(towerSelectedToDeploy);
        }
        else
        {
            DeployUnit(GetUnitAfterMergeCheck(towerSelectedToDeploy));
            SpawnParticles(1, -90);
        }

    }

    void SpawnParticles(int particleIndex, int rotation)
    {
        ParticleSystem particleTemp = Instantiate(MainPlayerControl.Instance.towerParticles[particleIndex], transform.position, Quaternion.Euler(rotation, 0f, 0f));
        Destroy(particleTemp.gameObject, particleTemp.main.duration);
    }
    public PlayerUnit GetUnitAfterMergeCheck(PlayerUnit towerSelectedToDeploy)
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
                if (towerSelectedToDeploy.unitPrefab.TowerAttackType == existingUnitCombination.combinesWith)
                {
                    PlayerUnit combinedTower = _mainPlayerControl.GetPlayerUnit(existingUnitCombination.toYield);

                    if (!_dataManager.IsAttackTypeUnlocked(combinedTower.unitType)) break;
                    return combinedTower;
                }
            }
        }
        Debug.Log("No possible merge combinations found for: " + towerSelectedToDeploy);
        return towerSelectedToDeploy;

    }

    public void DeployUnit(PlayerUnit towerSelectedToDeploy)
    {
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
                renderer.material.color = Color.clear;
            }
        }
    }

}
