using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionMenu : MonoBehaviour
{
    private MainPlayerControl playerControl;
    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
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
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
    }

    public void WindUnitSelected()
    {
        playerControl.activeUnitDeploymentArea.DeployAttackUnit(AttackType.WindAttack);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
    }
}
