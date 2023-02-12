using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;
    [Header("Readonly Components")]
    [ReadOnly] public List<PlayerTower> activePlayerTowersList = new();
    [ReadOnly] public PlayerUnitDeploymentArea activeUnitDeploymentArea;

    [Header("ATTACK UNITS"), Space(2)]
    public AttackUnit[] allAttackUnits;

    [Header("RESOURCE METER")]  //RENAME THIS  BLOCK LATER TO WHAT WE ARE USING FOR THE NAME OF RESOURCE
    [Range(1, 20)] public float maxResources = 10;
    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f; //Recharge Rate per second
    [ReadOnly, Range(1, 20)] public float currentResourcesCount = 10;
    private bool isRecharging = false;


    [Serializable]
    public class PlayerUnit
    {
        public AttackType unitType;
        public GameObject unitPrefab;
    }

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
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
    public AttackUnit GetAttackUnitObject(AttackType unitType)
    {
        foreach (AttackUnit unit in allAttackUnits)
        {
            if (unit.attackType == unitType)
                return unit;
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
        }
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
        isRecharging = false;
    }

    public void RemoveResource(int amount)
    {
        currentResourcesCount -= amount;
        currentResourcesCount = Mathf.Clamp(currentResourcesCount, 0, maxResources);
    }

    #endregion
}

public enum AttackType
{
    FireAttack,
    WindAttack,
    LightningAttack
}
public enum AttackUnitState
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