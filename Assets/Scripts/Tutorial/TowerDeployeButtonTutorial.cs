using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

public class TowerDeployeButtonTutorial : DraggableButton
{
    [Space(2), Header("DRAGGABLE BUTTON EXTENDED"), Space(2)]
    public AttackType attackType;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject rangeVisualObjPrefab;

    [Space(2), Header("Readonly")]
    [SerializeField, ReadOnly] private PlayerUnitDeploymentArea activeDeploymentArea;
    private MainPlayerControl mainPlayerControl;
    private UIManager uiManager;
    private GameObject spawnedRangeVisualObj;
    private bool initialized = false;
    public bool ableToDrag = true;

    private bool ResourcesAvailable
    {
        get
        {
            return mainPlayerControl.GetPlayerUnit(attackType).unitPrefab.resourceCost < mainPlayerControl.currentResourcesCount;
        }
    }

    private void Start()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        uiManager = UIManager.Instance;
        initialized = false;
        InitializeButton();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!ableToDrag)
            return;
        if (!ResourcesAvailable) return;

        base.OnBeginDrag(eventData);

        #region Tutorial Stuff
        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.completedTutorialSteps == TutorialManager.TutorialStep.STEP_ONE)
            {
                tutorialManager.blackPanel.SetActive(false);
                tutorialManager.deployAreas[0].transform.GetComponent<Renderer>().material.color = new Color32(0, 106, 2, 255);
            }


            #endregion
        }
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
            uiManager.ShowWarningText = mainPlayerControl.GetPlayerUnit(attackType).unitPrefab.attackType.ToString() +
                "Unit Needs: " + mainPlayerControl.GetPlayerUnit(attackType).unitPrefab.resourceCost.ToString() + " Gems";
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


        #region Tutorial Stuff

        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.completedTutorialSteps == TutorialManager.TutorialStep.STEP_ONE)
            {
                if (!activeDeploymentArea)
                {
                    tutorialManager.blackPanel.SetActive(true);
                    tutorialManager.deployAreas[0].gameObject.SetActive(false);
                }
                else
                {
                    tutorialManager.blackPanel.SetActive(false);
                    StartCoroutine(tutorialManager.TutorialSecondStep());
                    tutorialManager.deployAreas[0].transform.GetComponent<Renderer>().material.color = new Color32(0, 106, 2, 0);

                }
            }
        }
        #endregion

        ResetButton();
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

        InitializeButton(null);
    }

    void InitializeButton(PlayerUnitDeploymentArea possibleDeploymentArea = null)
    {
        if (possibleDeploymentArea != null)
        {
            if (initialized) return;
            initialized = true;

            activeDeploymentArea = possibleDeploymentArea;

            PlayerUnit possibleTower = mainPlayerControl.GetPlayerUnit(attackType);
            if (possibleTower == null) return;

            buttonIcon.sprite = possibleTower.deployButtonSprite;
            costText.text = possibleTower.unitPrefab.resourceCost.ToString();
        }
        else
        {
            PlayerUnit defaultPlayerUnit = mainPlayerControl.GetPlayerUnit(attackType);
            buttonIcon.sprite = defaultPlayerUnit.deployButtonSprite;
            costText.text = defaultPlayerUnit.unitPrefab.resourceCost.ToString();
        }
    }
    void HandleRangeVisuaizer(PlayerUnitDeploymentArea possibleDeploymentArea)
    {

        if (!spawnedRangeVisualObj) spawnedRangeVisualObj = Instantiate(rangeVisualObjPrefab);
        if (spawnedRangeVisualObj)
        {
            if (possibleDeploymentArea.HasDeployedUnit)
            {
                Destroy(spawnedRangeVisualObj);
                return;
            }

            PlayerUnit possibleTower = mainPlayerControl.GetPlayerUnit(attackType);

            spawnedRangeVisualObj.transform.position = possibleDeploymentArea.transform.position + new Vector3(0, 0.1f, 0);
            //Multiplied Local Scale by 2 becuase we are dealing with radius in shoooting range,
            //But setting scale here, scale is on either side of pivot while radius extends on one side
            spawnedRangeVisualObj.transform.localScale =
                2 * possibleTower.unitPrefab.shootingRange * Vector3.one;
        }


    }
}
