using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTowerUI : MonoBehaviour
{
    [Space(2), Header("UNIT UI PROPERTIES")]
    [SerializeField] private Image currentUnitTypeImage;
    [SerializeField] private Image currentUnitTypeImageBG;
    [SerializeField] protected Image healthBarImage;
    [SerializeField, Range(0.1f, 5)] private float uiUpdateRate = 1;

    [Space(2), Header("UPGRADES")]
    [SerializeField] private UnitUpgradesButton upgradeButtonPrefab;
    [SerializeField] private CanvasGroup upgradesPanelGroup;
    [SerializeField] private Transform toggleUpgradesPanelButton;
    [SerializeField] private Transform upgradeButtonsSpawnParent;
    [SerializeField] private Transform sourceComponentsParent;
    [SerializeField] private Image sourceComponentIcon01;
    [SerializeField] private Image sourceComponentIcon02;


    [SerializeField, ReadOnly] private int leastUpgradeCost;
    [SerializeField, ReadOnly] private float timer = 0;


    public Image HealthBarImage { get { return healthBarImage; } }
    public Image CurrentUnitTypeImage { get { return currentUnitTypeImage; } }
    public CanvasGroup UpgradesPanelGroup { get { return upgradesPanelGroup; } }

    private PlayerTower ownerTower;
    private MainPlayerControl _mainPlayerControl;

    public void InitializeUI(PlayerTower owner, Sprite unitTypeImage)
    {
        ownerTower = owner;
        _mainPlayerControl = owner._mainPlayerControl;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 ownerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, owner.transform.position);
        rectTransform.position = ownerScreenPos;

        if (ownerTower.playerUnitProperties.supportsCombining)
        {
            foreach (var combinations in owner.playerUnitProperties.possibleCombinations)
            {
                // Initialize all the upgrades buttons
                if (_mainPlayerControl.IsAttackTypeUnlocked(combinations.toYield))
                {
                    var upgradesButton = Instantiate(upgradeButtonPrefab, upgradeButtonsSpawnParent);
                    upgradesButton.InitializeButton(_mainPlayerControl.GetPlayerUnit(combinations.combinesWith),
                        _mainPlayerControl.GetPlayerUnit(combinations.toYield), owner);
                }

                //Calculate the least resources required for upgrades
                PlayerUnit unit = owner._mainPlayerControl.GetPlayerUnit(combinations.toYield);
                if (unit.resourceCost == 0 || unit.resourceCost > leastUpgradeCost)
                {
                    leastUpgradeCost = unit.resourceCost;
                }
            }
            sourceComponentsParent.gameObject.SetActive(false);
        }
        else
        {
            toggleUpgradesPanelButton.gameObject.SetActive(false);

            sourceComponentIcon01.sprite = ownerTower.playerUnitProperties.sourceComponent01;
            sourceComponentIcon02.sprite = ownerTower.playerUnitProperties.sourceComponent02;

            sourceComponentsParent.gameObject.SetActive(true);
        }

        currentUnitTypeImage.sprite = unitTypeImage;
        currentUnitTypeImageBG.sprite = unitTypeImage;



        ((RectTransform)transform).localScale = Vector3.zero;

        //Tweening Animations
        upgradesPanelGroup.DOFade(1, 0.5f);
        ((RectTransform)transform).DOScale(Vector3.one * 1.25f, 0.15f).OnComplete(() =>
            ((RectTransform)transform).DOScale(Vector3.one, 0.15f));

        timer = uiUpdateRate;
    }


    //BUTTON REFRENCE
    public virtual void ToggleUpgradesPanel()
    {
        if (!upgradesPanelGroup.gameObject.activeSelf)
        {
            upgradesPanelGroup.gameObject.SetActive(true);
            ((RectTransform)upgradesPanelGroup.transform).localScale = Vector3.zero;
            upgradesPanelGroup.alpha = 0;

            //Tweening Animations
            upgradesPanelGroup.DOFade(1, 0.25f);
            ((RectTransform)upgradesPanelGroup.transform).DOScale(Vector3.one, 0.25f);
        }
        else
        {
            ((RectTransform)upgradesPanelGroup.transform).localScale = Vector3.one;
            upgradesPanelGroup.alpha = 1;

            //Tweening Animations
            upgradesPanelGroup.DOFade(0, 0.25f);
            ((RectTransform)upgradesPanelGroup.transform).DOScale(Vector3.zero, 0.25f).OnComplete(()
                => upgradesPanelGroup.gameObject.SetActive(false));
        }
    }
}
