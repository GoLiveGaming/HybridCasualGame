using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDeployButton : DraggableButton
{
    [Space(2), Header("DRAGGABLE BUTTON EXTENDED"), Space(2)]
    [SerializeField] private AttackType attackType;
    [SerializeField] private GameObject rangeVisualObjPrefab;

    [Space(2), Header("Readonly")]
    [SerializeField, ReadOnly] private PlayerUnitDeploymentArea activeDeploymentArea;
    private MainPlayerControl mainPlayerControl;
    private GameObject spawnedRangeVisualObj;

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);


        Ray ray = Camera.main.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 200, layersToCollide))
        {
            if (hit.transform.TryGetComponent(out PlayerUnitDeploymentArea possibleDeploymentArea))
            {
                Debug.Log(possibleDeploymentArea.transform.name + " Detected");

                if (possibleDeploymentArea != activeDeploymentArea)
                {
                    Clear();
                    possibleDeploymentArea.ToggleHighlightArea(true);
                    activeDeploymentArea = possibleDeploymentArea;
                }
                HandleRangeVisuaizer(possibleDeploymentArea);
                return;

            }
            Clear();
        }

    }

    void HandleRangeVisuaizer(PlayerUnitDeploymentArea possibleDeploymentArea)
    {

        if (!spawnedRangeVisualObj) spawnedRangeVisualObj = Instantiate(rangeVisualObjPrefab);
        if (spawnedRangeVisualObj)
        {
            spawnedRangeVisualObj.transform.position = possibleDeploymentArea.transform.position + new Vector3(0, 0.1f, 0);
            //Multiplied Local Scale by 2 becuase we are dealing with radius in shoooting range,
            //But setting scale here, scale is on either side of pivot while radius extends on one side
            spawnedRangeVisualObj.transform.localScale =
                2 * possibleDeploymentArea.GetUnitAfterMergeCheck(mainPlayerControl.
                GetAttackUnitObject(attackType)).shootingRange * Vector3.one;
        }


    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (activeDeploymentArea)
            activeDeploymentArea.DeployAttackUnit(attackType);

        Clear();
    }

    void Clear()
    {
        if (activeDeploymentArea)
        {
            activeDeploymentArea.ToggleHighlightArea(false);
        }

        if (spawnedRangeVisualObj)
            Destroy(spawnedRangeVisualObj);


        activeDeploymentArea = null;
    }
}
