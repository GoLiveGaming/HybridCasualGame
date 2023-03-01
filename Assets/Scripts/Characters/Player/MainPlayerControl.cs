using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Header("ATTACK UNITS"), Space(2)]
    public PlayerUnit[] allPlayerUnits;

    public ParticleSystem[] towerParticles;
    public ParticleSystem[] enemyParticles;

    [Header("RESOURCE METER")]                                          //RENAME THIS  BLOCK LATER TO WHAT WE ARE USING FOR THE NAME OF RESOURCE
    [Range(1, 20)] public float maxResources = 10;
    [Range(0.1f, 5f)] public float resourceRechargeRate = 1.0f;         //Recharge Rate per second

    [Space(2), Header("READONLY")]
    [ReadOnly, Range(1, 20)] public float currentResourcesCount = 10;
    [ReadOnly] public List<PlayerUnitBase> activePlayerTowersList = new();
    [ReadOnly] public List<PlayerMainTower> mainPlayerTower = new();
    [ReadOnly] public bool isRecharging = false;
    private UIManager uiManager;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;

    }
    void Update()
    {
        UpdateResourceMeter();
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
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
