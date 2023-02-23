using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDeployButton : DraggableButton
{
    [Space(2), Header("DRAGGABLE BUTTON EXTENDED"), Space(2)]
    [SerializeField] private AttackType attackType;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject rangeVisualObjPrefab;

    [Space(2), Header("Readonly")]
    [SerializeField, ReadOnly] private PlayerUnitDeploymentArea activeDeploymentArea;
    private MainPlayerControl mainPlayerControl;
    private GameObject spawnedRangeVisualObj;
    private bool initialized = false;

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        initialized = false;
        InitilizeButton();
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
                    ResetButton();
                    ReInitializeButton(possibleDeploymentArea);
                    possibleDeploymentArea.ToggleHighlightArea(true);
                }
                HandleRangeVisuaizer(possibleDeploymentArea);
                return;
            }

        }
        ResetButton();

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (activeDeploymentArea)
            activeDeploymentArea.DeployAttackUnit(attackType);

        ResetButton();
    }
    void ReInitializeButton(PlayerUnitDeploymentArea possibleDeploymentArea)
    {
        if (initialized) return;
        initialized = true;

        if (!possibleDeploymentArea) return;

        activeDeploymentArea = possibleDeploymentArea;

        InitilizeButton(possibleDeploymentArea);

    }
    void ResetButton()
    {
        if (!initialized) return;
        initialized = false;

        if (activeDeploymentArea)
        {
            activeDeploymentArea.ToggleHighlightArea(false);
        }

        if (spawnedRangeVisualObj)
            Destroy(spawnedRangeVisualObj);

        activeDeploymentArea = null;

        InitilizeButton(null);
    }

    void InitilizeButton(PlayerUnitDeploymentArea possibleDeploymentArea = null)
    {
        if (possibleDeploymentArea)
        {
            PlayerUnit possibleTower = possibleDeploymentArea.GetUnitAfterMergeCheck(mainPlayerControl.
                   GetPlayerUnit(attackType));
            buttonIcon.sprite = possibleTower.unitPrefab.TowerIcon;
            costText.text = possibleTower.unitPrefab.resourceCost.ToString();
        }
        else
        {
            PlayerUnit defaultPlayerUnit = mainPlayerControl.GetPlayerUnit(attackType);
            buttonIcon.sprite = defaultPlayerUnit.unitPrefab.TowerIcon;
            costText.text = defaultPlayerUnit.unitPrefab.resourceCost.ToString();
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
                GetPlayerUnit(attackType)).unitPrefab.shootingRange * Vector3.one;
        }


    }
}
