using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Header("ATTACK UNITS"), Space(2)]
    public PlayerUnit[] allPlayerUnits;

    public ParticleSystem[] towerParticles;
    public ParticleSystem[] enemyParticles;

    [Header("UNIT UPGRADES"), Space(2)]
    [SerializeField] private LayerMask deployAreaLayer;
    [SerializeField] private UnitUpgradesButton upgradeButtonPrefab;

    [Header("RESOURCE METER")]                                          //RENAME THIS  BLOCK LATER TO WHAT WE ARE USING FOR THE NAME OF RESOURCE
    [Range(1, 20)] public float maxResources = 10;
    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f;         //Recharge Rate per second

    [Space(2), Header("READONLY")]
    [ReadOnly, Range(1, 20)] public float currentResourcesCount = 10;
    [ReadOnly] public List<PlayerUnitBase> activePlayerTowersList = new();
    [ReadOnly] public List<PlayerMainTower> mainPlayerTower = new();
    [ReadOnly] public PlayerDataManager _dataManager;
    [ReadOnly] public bool isRecharging = false;
    private UIManager uiManager;
    private Camera mainCamera;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;

        uiManager = UIManager.Instance;
        _dataManager = PlayerDataManager.Instance;
    }
    void Update()
    {
        HandleResources();
        HandleUnitUpgrades();
    }

    public PlayerUnit GetPlayerUnit(AttackType unitType)
    {
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            if (unit.unitType == unitType)
                return unit;
        }
        return null;
    }

    #region UPGRADES MANAGER
    private void HandleUnitUpgrades()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if the touch was on a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Cast a ray from the touch position in the specified direction
            Ray ray = mainCamera.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200, deployAreaLayer))
            {
                if (hit.transform.TryGetComponent(out PlayerUnitDeploymentArea area))
                {
                    if (!area.HasDeployedUnit)
                    {
                        uiManager.unitUpgradesPanel.SetActive(false);
                        return;
                    }

                    StartUpgradesProcess(area);
                }
            }
            else
            {
                uiManager.unitUpgradesPanel.SetActive(false);
            }
        }
    }

    private void StartUpgradesProcess(PlayerUnitDeploymentArea area)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(area.transform.position);

        foreach (Transform tf in uiManager.unitUpgradesPanel.transform)
        {
            Destroy(tf.gameObject);
        }
        foreach (MergingCombinations existingUnitCombination in area.deployedTower.possibleCombinations)
        {
            if (!_dataManager.IsAttackTypeUnlocked(existingUnitCombination.toYield)) continue;

            UnitUpgradesButton upgradesButton = Instantiate(upgradeButtonPrefab, uiManager.unitUpgradesPanel.transform);

            upgradesButton.InitializeButton(GetPlayerUnit(existingUnitCombination.toYield), area);
        }

        RectTransform rectTransform = uiManager.unitUpgradesPanel.GetComponent<RectTransform>();
        rectTransform.position = screenPosition + new Vector3(0, 400, 0);
        uiManager.unitUpgradesPanel.SetActive(true);
    }

    #endregion

    #region RESOURCE MANAGEMENT
    public void HandleResources()
    {
        if (currentResourcesCount < maxResources && !isRecharging)
        {
            StartCoroutine(RechargeResource());
        }
    }
    IEnumerator RechargeResource()
    {
        if (uiManager.resourceMeterAnimator) uiManager.resourceMeterAnimator.SetBool("Full", false);
        isRecharging = true;
        if (currentResourcesCount == 0 && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaOut);


        while (currentResourcesCount < maxResources)
        {
            yield return null;
            currentResourcesCount += resourceRechargeRate * Time.deltaTime;
            uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
            uiManager.resourcesCount.text = Mathf.RoundToInt(currentResourcesCount).ToString();

        }
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
        uiManager.resourcesCount.text = Mathf.RoundToInt(currentResourcesCount).ToString();
        if (currentResourcesCount == maxResources && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaFull);
        if (uiManager.resourceMeterAnimator) uiManager.resourceMeterAnimator.SetBool("Full", true);
        isRecharging = false;
    }

    public void RemoveResource(int amount)
    {
        currentResourcesCount -= amount;
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.resourceMeter.fillAmount = currentResourcesCount / maxResources;
    }

    #endregion
}
