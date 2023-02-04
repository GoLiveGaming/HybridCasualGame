using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitDeploymentArea : MonoBehaviour
{
    [SerializeField] private GameObject unitSelectionCanvas;
    public void OnUnitSelected()
    {
        unitSelectionCanvas.SetActive(true);
    }
}
