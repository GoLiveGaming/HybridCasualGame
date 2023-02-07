using DG.Tweening;
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
        MainPlayerControl.Instance.activeUnitDeploymentArea.DeployUnit(AttackType.FireAttack);
        this.gameObject.SetActive(false);
    }

    public void WindUnitSelected()
    {
        MainPlayerControl.Instance.activeUnitDeploymentArea.DeployUnit(AttackType.WindAttack);
        this.gameObject.SetActive(false);
    }
}
