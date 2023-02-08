using System.Collections;
using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField] private float unitReplaceCooldownTime = 5f;
    [SerializeField] private bool areaBusy;
    [SerializeField, ReadOnly] private PlayerTower deployedTower;

    private UIManager uiManager;

    private void Start()
    {
        uiManager = UIManager.Instance;
    }
    public void OnUnitSelected()
    {
        MainPlayerControl.Instance.activeUnitDeploymentArea = this;
        unitSelectionCanvas.SetActive(true);
    }

    public void DeployUnit(AttackType unitType)
    {
        if (areaBusy) return;
        StartCoroutine(StartUnitReplaceCooldown());

        if (deployedTower && deployedTower.attackUnit.attackType == unitType)
        {
            deployedTower.UpgradeTower();
        }
        else
        {
            DeleteChildAttackUnits();
            GameObject objectToSpawn = MainPlayerControl.Instance.GetUnitToSpawn(unitType);
            GameObject spawnedObject = Instantiate(objectToSpawn, this.transform.position, Quaternion.identity);
            spawnedObject.transform.SetParent(transform, true);
            deployedTower = spawnedObject.GetComponent<PlayerTower>();
        }
    }

    private void DeleteChildAttackUnits()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator StartUnitReplaceCooldown()
    {
        areaBusy = true;

        float timer = 0;
        while (timer < unitReplaceCooldownTime)
        {
            timer += Time.deltaTime;
            uiManager.unitSelectionCooldownTimerImage.fillAmount = timer / unitReplaceCooldownTime;
            yield return null;
        }
        areaBusy = false;
    }
}
