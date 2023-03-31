using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTowerUI : MonoBehaviour
{
    [SerializeField] private Image currentUnitTypeImage;
    [SerializeField] private Image currentUnitTypeImageBG;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image sourceIndicatorIcon01;
    [SerializeField] private Image sourceIndicatorIcon02;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private int leastUpgradeCost;
    [SerializeField] private CanvasGroup upgradesPanelGroup;
    [SerializeField] private Transform upgradeButtonsParent;
    [SerializeField] private Transform sourceIndicatorIconsParent;

    [SerializeField, Range(0.1f, 5)] private float uiUpdateRate = 1;
    [SerializeField, ReadOnly] private float timer = 0;

    public Image CurrentUnitTypeImage { get { return currentUnitTypeImage; } }
    public Image HealthBarImage { get { return healthBarImage; } }
    public CanvasGroup UpgradesPanelGroup { get { return upgradesPanelGroup; } }

    private PlayerTower ownerTower;
    private MainPlayerControl _mainPlayerControl;


    public void InitializeStatsUI(PlayerTower owner, Sprite unitTypeImage)
    {
        ownerTower = owner;
        _mainPlayerControl = owner._mainPlayerControl;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 ownerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, owner.transform.position);
        rectTransform.position = ownerScreenPos;

        if (ownerTower.supportsCombining)
        {
            foreach (var combinations in owner.possibleCombinations)
            {
                // Initialize all the upgrades buttons
                if (_mainPlayerControl.IsAttackTypeUnlocked(combinations.toYield))
                {
                    var upgradesButton = Instantiate(owner._uiManager.upgradeButtonPrefab, upgradeButtonsParent);
                    upgradesButton.InitializeButton(_mainPlayerControl.GetPlayerUnit(combinations.combinesWith),
                        _mainPlayerControl.GetPlayerUnit(combinations.toYield), owner);
                }

                //Calculate the least resources required for upgrades
                PlayerUnit unit = owner._mainPlayerControl.GetPlayerUnit(combinations.toYield);
                if (unit.unitPrefab.resourceCost == 0 || unit.unitPrefab.resourceCost > leastUpgradeCost)
                {
                    leastUpgradeCost = unit.unitPrefab.resourceCost;
                }
            }
            sourceIndicatorIconsParent?.gameObject.SetActive(false);
        }
        else
        {
            upgradeButton.gameObject.SetActive(false);

            bool flag = false;
            foreach (var unit_01 in _mainPlayerControl.allPlayerUnits)
            {
                foreach (var unit_02 in _mainPlayerControl.allPlayerUnits)
                {
                    PlayerUnit mergedUnit = _mainPlayerControl.CanUnitsBeMerged(unit_01, unit_02);
                    if (mergedUnit != null && mergedUnit == owner.playerUnitProperties)
                    {
                        sourceIndicatorIcon01.sprite = unit_01.indicatorColorIcon;
                        sourceIndicatorIcon02.sprite = unit_02.indicatorColorIcon;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }

            sourceIndicatorIconsParent?.gameObject.SetActive(true);
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


    public void UpdateUI()
    {
        if (timer < uiUpdateRate)
        {
            timer += Time.deltaTime;
            return;
        }
        else
            timer = 0;


        if (!ownerTower) return;
        if (!ownerTower.supportsCombining) return;

        if (_mainPlayerControl.currentResourcesCount >= leastUpgradeCost)
        {
            if (!upgradeButton.activeSelf)
            {
                ((RectTransform)upgradeButton.transform).localScale = Vector3.zero;

                upgradeButton.gameObject.SetActive(true);

                ((RectTransform)upgradeButton.transform).DOScale(Vector3.one * 1.25f, 0.15f).OnComplete(() =>
            ((RectTransform)upgradeButton.transform).DOScale(Vector3.one, 0.15f));
            }
        }
        else
        {
            if (upgradeButton.activeSelf)
            {
                ((RectTransform)upgradeButton.transform).DOScale(Vector3.zero, 0.25f).OnComplete(()
                    => upgradeButton.gameObject.SetActive(false));
            }
        }
    }


    //BUTTON REFRENCE
    public void ToggleUpgradesPanel()
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
