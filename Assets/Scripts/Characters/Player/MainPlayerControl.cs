using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerControl : MonoBehaviour
{
    public static MainPlayerControl Instance;

    [Header("ATTACK UNITS"), Space(2)]
    public PlayerTower[] allPlayerTowers;

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
        uiManager = UIManager.Instance;
    }
    void Update()
    {
        UpdateResourceMeter();
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
        if(currentResourcesCount == 0 && AudioManager.Instance)
           AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaOut);
        

        while (currentResourcesCount < maxResources)
        {
            yield return new WaitForSeconds(1f);
            currentResourcesCount += resourceRechargeRate;
            uiManager.unitSelectionCooldownTimerImage.fillAmount = currentResourcesCount / maxResources;
            if(currentResourcesCount == maxResources && AudioManager.Instance)
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.ManaFull);
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