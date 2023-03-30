using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

public class TowerDeployButton : DraggableButton
{
    [Space(2), Header("DRAGGABLE BUTTON EXTENDED"), Space(2)]
    [SerializeField] private AttackType attackType;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject rangeVisualObjPrefab;

    [Space(2), Header("Readonly")]
    [SerializeField, ReadOnly] private PlayerUnitDeploymentArea activeDeploymentArea;
    private MainPlayerControl _mainPlayerControl;
    private UIManager _uiManager;
    private GameObject _spawnedRangeVisualObj;
    private bool _initialized = false;
    public bool ableToDrag = true;

    private bool ResourcesAvailable
    {
        get
        {
            return _mainPlayerControl.GetPlayerUnit(attackType).unitPrefab.resourceCost < _mainPlayerControl.currentResourcesCount;
        }
    }

    private void Start()
    {
        _mainPlayerControl = MainPlayerControl.Instance;
        _uiManager = UIManager.Instance;
        _initialized = false;
        InitializeButton();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!ableToDrag)
            return;
        if (!ResourcesAvailable) return;

        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (!ableToDrag)
            return;
        if (!ResourcesAvailable) return;
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
                    InitializeButton(possibleDeploymentArea);
                    possibleDeploymentArea.ToggleHighlightArea(true);

                }
                HandleRangeVisuaizer(possibleDeploymentArea);
                return;
            }
        }
        else
        {
            ResetButton();
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!ableToDrag)
            return;
        if (!ResourcesAvailable)
        {
            _uiManager.ShowNotEnoughResourcesEffect();
            return;
        }

        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!ableToDrag)
            return;

        if (!ResourcesAvailable) return;

        base.OnPointerUp(eventData);

        if (activeDeploymentArea)
            activeDeploymentArea.DeployAttackUnit(attackType);

        ResetButton();
    }

    void ResetButton()
    {
        if (!_initialized) return;
        _initialized = false;

        if (activeDeploymentArea)
        {
            activeDeploymentArea.ToggleHighlightArea(false);
        }

        if (_spawnedRangeVisualObj)
            Destroy(_spawnedRangeVisualObj);

        activeDeploymentArea = null;

        InitializeButton(null);
    }

    void InitializeButton(PlayerUnitDeploymentArea possibleDeploymentArea = null)
    {

        if (possibleDeploymentArea != null)
        {
            if (_initialized) return;
            _initialized = true;

            activeDeploymentArea = possibleDeploymentArea;

            PlayerUnit possibleTower = _mainPlayerControl.GetPlayerUnit(attackType);
            if (possibleTower == null) return;

            buttonIcon.sprite = possibleTower.deployButtonSprite;
            costText.text = possibleTower.unitPrefab.resourceCost.ToString();
        }
        else
        {
            PlayerUnit defaultPlayerUnit = _mainPlayerControl.GetPlayerUnit(attackType);
            buttonIcon.sprite = defaultPlayerUnit.deployButtonSprite;
            costText.text = defaultPlayerUnit.unitPrefab.resourceCost.ToString();
        }
    }
    void HandleRangeVisuaizer(PlayerUnitDeploymentArea possibleDeploymentArea)
    {

        if (!_spawnedRangeVisualObj) _spawnedRangeVisualObj = Instantiate(rangeVisualObjPrefab);
        if (_spawnedRangeVisualObj)
        {
            if (possibleDeploymentArea.HasDeployedUnit)
            {
                Destroy(_spawnedRangeVisualObj);
                return;
            }

            PlayerUnit possibleTower = _mainPlayerControl.GetPlayerUnit(attackType);

            _spawnedRangeVisualObj.transform.position = possibleDeploymentArea.transform.position + new Vector3(0, 0.1f, 0);
            //Multiplied Local Scale by 2 becuase we are dealing with radius in shoooting range,
            //But setting scale here, scale is on either side of pivot while radius extends on one side
            _spawnedRangeVisualObj.transform.localScale =
                2 * possibleTower.unitPrefab.shootingRange * Vector3.one;
        }


    }
}
