using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField] private bool areaBusy;
    public void OnUnitSelected()
    {
        MainPlayerControl.instance.activeUnitDeploymentArea = this;
        unitSelectionCanvas.SetActive(true);
    }

    public void DeployUnit(MainPlayerControl.PlayerUnitType unitType)
    {
        if (transform.childCount != 0) areaBusy = true;
        else areaBusy = false;
        if (!areaBusy)
        {
            GameObject objectToSpawn = MainPlayerControl.instance.GetUnitToSpawn(unitType);
            Instantiate(objectToSpawn, this.transform.position, Quaternion.identity, this.transform);
        }
    }
}
