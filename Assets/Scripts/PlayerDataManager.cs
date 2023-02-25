using System;
using UnityEditor;
using UnityEngine;

public class PlayerDataManager : SingletonPersistent<PlayerDataManager>
{

    private readonly PlayerDataContainer _playerData = new();
    public PlayerDataContainer PlayerData { get { return _playerData; } }

    private void Start()
    {
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

            PlayerPrefs.SetInt("AttackTypesList_Count", _playerData.AllAttackTypesData.Count);

            PlayerPrefs.SetInt("Initialized", 1);
        }
        UpdateAttackTypesListsFromPlayerPrefs();
        PlayerPrefs.Save();
    }


    private void UpdateAttackTypesListsFromPlayerPrefs()
    {

        foreach (AttackType value in Enum.GetValues(typeof(AttackType)))
        {
            PlayerAttacksData attackTypeData = new();

            attackTypeData.AttackType = value;

            if (PlayerPrefs.GetInt("AttackType_" + value.ToString()) == 1)
                attackTypeData.isUnlocked = 1;

            _playerData.AllAttackTypesData.Add(attackTypeData);
        }

    }

    #region Global Access Functions

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
            attackType = AttackType.FireAttack;
        }
        return attackType;
    }
    public bool UnlockAttackType(string attackTypeName)
    {
        if (PlayerPrefs.HasKey("AttackType_" + attackTypeName))
        {
            PlayerPrefs.SetInt("AttackType_" + attackTypeName, 1);
        }
        else return false;
        UpdateAttackTypesListsFromPlayerPrefs();
        PlayerPrefs.Save();
        return true;
    }

    #endregion
}