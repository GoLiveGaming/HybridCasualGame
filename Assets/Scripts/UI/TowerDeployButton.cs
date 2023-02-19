using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDeployButton : DraggableButton
{
    [Space(2), Header("DRAGGABLE BUTTON EXTENDED"), Space(2)]
    [SerializeField] private AttackType attackType;

    [Space(2), Header("Readonly")]
    [SerializeField, ReadOnly] private PlayerUnitDeploymentArea deploymentArea;
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);


        Ray ray = Camera.main.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 200, layersToCollide))
        {
            if (hit.transform.TryGetComponent(out PlayerUnitDeploymentArea playerUnitDeploymentArea))
            {
                Debug.Log(playerUnitDeploymentArea.transform.name + " Selected");
                deploymentArea = playerUnitDeploymentArea;
            }
            else
            {
                Debug.Log("No Deployment Selected");
                deploymentArea = null;
            }
        }

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (deploymentArea)
            deploymentArea.DeployAttackUnit(attackType);

        deploymentArea = null;
    }
}
