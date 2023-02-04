using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField] private bool areaHasUnit;
    public void OnUnitSelected()
    {
        unitSelectionCanvas.SetActive(true);
    }

    public void UnitDeployedInArea()
    {
        if (!areaHasUnit) { }
    }
}
