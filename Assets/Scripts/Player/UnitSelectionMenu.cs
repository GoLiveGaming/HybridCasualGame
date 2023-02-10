using DG.Tweening;
using UnityEngine;

public class UnitSelectionMenu : MonoBehaviour
{
    private MainPlayerControl playerControl;
    private void Start()
    {
        gameObject.SetActive(false);
        playerControl = MainPlayerControl.Instance;
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
        playerControl.activeUnitDeploymentArea.DeployAttackUnit(AttackType.FireAttack);
        this.gameObject.SetActive(false);
    }

    public void WindUnitSelected()
    {
        playerControl.activeUnitDeploymentArea.DeployAttackUnit(AttackType.WindAttack);
        this.gameObject.SetActive(false);
    }
}
