using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerDataManager : SingletonPersistent<PlayerDataManager>
{

    private readonly PlayerDataContainer _playerData = new();
    public PlayerDataContainer PlayerData { get { return _playerData; } }

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
            PlayerPrefs.SetInt("AttackTypesList_Count", _playerData.AllAttackTypesData.Count);



            PlayerPrefs.SetInt("Initialized", 1);
        }
        UpdateDataFromPlayerPrefs();
        PlayerPrefs.Save();
    }


    private void UpdateDataFromPlayerPrefs()
    {
        _playerData.AllAttackTypesData.Clear();

        foreach (AttackType value in Enum.GetValues(typeof(AttackType)))
        {
            PlayerAttacksData attackTypeData = new();

            attackTypeData.AttackType = value;

            if (PlayerPrefs.GetInt("AttackType_" + value.ToString()) == 1)
                attackTypeData.isUnlocked = 1;

            _playerData.AllAttackTypesData.Add(attackTypeData);
        }

        _playerData.coinsAmount = PlayerPrefs.GetInt("CoinsAmount");

    }

    #region Global Access Functions

    public int CoinsAmount
    {
        get { return _playerData.coinsAmount; }
        set
        {
            _playerData.coinsAmount = value;

            PlayerPrefs.SetInt("CoinsAmount", _playerData.coinsAmount);
        }
    }
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }



    public bool IsAttackTypeUnlocked(AttackType type)
    {
        foreach (PlayerAttacksData data in _playerData.AllAttackTypesData)
        {
            if (data.AttackType == type)
            {
                return data.isUnlocked > 0;
            }
        }

        return false;
    }
    public AttackType GetAttackTypeFromString(string attackTypeName)
    {
        AttackType attackType;
        try
        {
            attackType = (AttackType)Enum.Parse(typeof(AttackType), attackTypeName);
        }
        catch (ArgumentException)
        {
            Debug.LogError("Invalid enum name: " + name);
            attackType = AttackType.NONE;
        }
        return attackType;
    }
    public bool UnlockAttackType(string attackTypeName)
    {
        if (CoinsAmount <= 0) return false;

        if (PlayerPrefs.HasKey("AttackType_" + attackTypeName))
        {
            PlayerPrefs.SetInt("AttackType_" + attackTypeName, 1);
        }
        else return false;
        CoinsAmount -= 1;
        UpdateDataFromPlayerPrefs();
        PlayerPrefs.Save();
        return true;
    }

    #endregion
}