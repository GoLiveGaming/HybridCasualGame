using System;
using UnityEngine;

public class PlayerDataManager : SingletonPersistent<PlayerDataManager>
{

    private readonly PlayerDataContainer _playerDataInstance = new();
    public PlayerDataContainer PlayerData { get { return _playerDataInstance; } }

    protected override void Awake()
    {
        base.Awake();

        if (!PlayerPrefs.HasKey("Initialized") || PlayerPrefs.GetInt("Initialized") == 0)
        {
            foreach (AttackType value in Enum.GetValues(typeof(AttackType)))
            {
                PlayerPrefs.SetInt("AttackType_" + value.ToString(), 0);
            }
            PlayerPrefs.SetInt("AttackType_" + AttackType.FireAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.WindAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.WaterAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.LightningAttack.ToString(), 1);

            PlayerPrefs.SetInt("CoinsAmount", 0);
            PlayerPrefs.SetInt("AttackTypesList_Count", _playerDataInstance.AllAttackTypesData.Count);

            PlayerPrefs.SetInt("Initialized", 1);
        }
        UpdateDataFromPlayerPrefs();
        PlayerPrefs.Save();
    }


    private void UpdateDataFromPlayerPrefs()
    {
        _playerDataInstance.AllAttackTypesData.Clear();

        foreach (AttackType value in Enum.GetValues(typeof(AttackType)))
        {
            PlayerAttacksData attackTypeData = new();

            attackTypeData.AttackType = value;

            if (PlayerPrefs.GetInt("AttackType_" + value.ToString()) == 1)
                attackTypeData.isUnlocked = 1;

            _playerDataInstance.AllAttackTypesData.Add(attackTypeData);
        }

        _playerDataInstance.coinsAmount = PlayerPrefs.GetInt("CoinsAmount");

    }
    private void SaveDataToPlayerPrefs()
    {
        //Update Attack Types in Playerprefs
        foreach (PlayerAttacksData data in _playerDataInstance.AllAttackTypesData)
        {
            if (data.isUnlocked == 1)
            {
                PlayerPrefs.SetInt("AttackType_" + data.AttackType.ToString(), 1);
            }
        }

        //Update Coins Amount in PlayerPrefs
        PlayerPrefs.SetInt("CoinsAmount", _playerDataInstance.coinsAmount);


        PlayerPrefs.Save();
    }

    #region Global Access Functions

    public int CoinsAmount
    {
        get { return _playerDataInstance.coinsAmount; }
        set { _playerDataInstance.coinsAmount = value; }
    }
    public bool UnlockAttackType(AttackType attackType)
    {
        if (CoinsAmount <= 0) return false;
        if (IsAttackTypeUnlocked(attackType)) return false;

        foreach (var data in _playerDataInstance.AllAttackTypesData)
        {
            if (data.AttackType == attackType)
            {
                data.isUnlocked = 1;
                CoinsAmount -= 1;
                SaveDataToPlayerPrefs();

                return true;
            }
        }
        return false;
    }
    public bool IsAttackTypeUnlocked(AttackType type)
    {
        foreach (PlayerAttacksData data in _playerDataInstance.AllAttackTypesData)
        {
            if (data.AttackType == type)
            {
                return data.isUnlocked > 0;
            }
        }

        return false;
    }
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion
}