using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Header("ATTACK UNITS"), Space(2)]
    public PlayerTower[] allPlayerTowers;

    [Header("RESOURCE METER")]                                          //RENAME THIS  BLOCK LATER TO WHAT WE ARE USING FOR THE NAME OF RESOURCE
    [Range(1, 20)] public float maxResources = 10;
    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f;         //Recharge Rate per second

    [Space(2), Header("READONLY")]
    [ReadOnly, Range(1, 20)] public float currentResourcesCount = 10;
    [ReadOnly] public List<PlayerUnitBase> activePlayerTowersList = new();
    [ReadOnly] public PlayerUnitDeploymentArea activeUnitDeploymentArea;
    [ReadOnly] public bool isRecharging = false;
    private UIManager uiManager;


    [Serializable]
    public class PlayerUnit
    {
        public AttackType unitType;
        public GameObject unitPrefab;
    }

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
    }
    void Update()
    {
        uiManager = UIManager.Instance;

        UpdateInputs();
        UpdateResourceMeter();
    }
    private void UpdateInputs()
    {
        if (Input.GetButtonDown("Fire1")) SelectUnitDeploymentArea();
    }
    void SelectUnitDeploymentArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            if (hit.transform.gameObject.TryGetComponent(out PlayerUnitDeploymentArea playerUnitDeploymentArea))
            {
                playerUnitDeploymentArea.OnUnitSelectionStarted();
            }
        }
    }
    public PlayerTower GetAttackUnitObject(AttackType unitType)
    {
        foreach (PlayerTower tower in allPlayerTowers)
        {
            if (tower.TowerAttackType == unitType)
                return tower;
        }
        return null;
    }

    #region RESOURCE MANAGEMENT
    public void UpdateResourceMeter()
    {
        if (currentResourcesCount < maxResources && !isRecharging)
        {
            StartCoroutine(RechargeResource());
        }

    }
    IEnumerator RechargeResource()
    {
        isRecharging = true;

        while (currentResourcesCount < maxResources)
        {
            yield return new WaitForSeconds(1f);
            currentResourcesCount += resourceRechargeRate;
            uiManager.unitSelectionCooldownTimerImage.fillAmount = currentResourcesCount / maxResources;
        }
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.unitSelectionCooldownTimerImage.fillAmount = currentResourcesCount / maxResources;
        isRecharging = false;
    }

    public void RemoveResource(int amount)
    {
        currentResourcesCount -= amount;
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        uiManager.unitSelectionCooldownTimerImage.fillAmount = currentResourcesCount / maxResources;
    }

    #endregion
}

public enum AttackType
{
    FireAttack,
    WindAttack,
    WaterAttack,
    LightningAttack,
    IceAttack,
    ExplosionAttack,
    HellfireAttack,
    StormAttack,
    FloodAttack

}
public enum TowerState
{
    Idle,
    Attack,
    Destroyed
}

[Serializable]
public class MergingCombinations
{
    public AttackType combinesWith;
    public AttackType toYield;
}