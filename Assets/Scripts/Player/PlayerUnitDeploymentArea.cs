using System.Collections;
using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField] private float unitReplaceCooldownTime = 5f;
    [SerializeField] private bool areaBusy;

    private UIManager uiManager;

    private void Start()
    {
        uiManager = UIManager.Instance;
    }
    public void OnUnitSelected()
    {
        MainPlayerControl.instance.activeUnitDeploymentArea = this;
        unitSelectionCanvas.SetActive(true);
    }

    public void DeployUnit(MainPlayerControl.AttackType unitType)
    {
        if (areaBusy) return;
        if (transform.childCount != 0) DeleteChildAttackUnits();

        StartCoroutine(StartUnitReplaceCooldown());

        GameObject objectToSpawn = MainPlayerControl.instance.GetUnitToSpawn(unitType);
        GameObject spawnedObject = Instantiate(objectToSpawn, this.transform.position, Quaternion.identity);
        spawnedObject.transform.SetParent(transform, true);
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
