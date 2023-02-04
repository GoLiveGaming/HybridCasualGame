using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionMenu : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        transform.DOScale(Vector3.one, 0.25f);
    }
    private void OnDisable()
    {
        transform.DOScale(Vector3.zero, 0.1f);
    }

    public void FireUnitSelected()
    {
        MainPlayerControl.instance.activeUnitDeploymentArea.DeployUnit(MainPlayerControl.PlayerUnitType.FireAttackUnit);
        this.gameObject.SetActive(false);
    }

    public void WindUnitSelected()
    {
        MainPlayerControl.instance.activeUnitDeploymentArea.DeployUnit(MainPlayerControl.PlayerUnitType.WindAttackUnit);
        this.gameObject.SetActive(false);
    }
}
