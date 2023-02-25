using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : SingletonPersistent<PlayerDataManager>
{

    private readonly PlayerDataContainer _playerDataContainer = new();


    public PlayerDataContainer PlayerDataContainer { get { return _playerDataContainer; } }



    private void Start()
    {
        if (!PlayerPrefs.HasKey("Initialized") || PlayerPrefs.GetInt("Initialized") == 0)
        {

            foreach (AttackType value in System.Enum.GetValues(typeof(AttackType)))
            {
                _playerDataContainer.AllAttackTypes.Add(value);
                PlayerPrefs.SetInt("AttackType_" + value.ToString(), 0);
            }

            PlayerPrefs.SetInt("AttackTypesList_Count", _playerDataContainer.AllAttackTypes.Count);

            PlayerPrefs.SetInt("Initialized", 1);
        }

        _playerDataContainer.AllAttackTypes = GetAllAttackTypesListFromPlayerPrefs();
        _playerDataContainer.UnlockedAttackTypes = GetUnlockedAttackTypesListFromPlayerPrefs();
        PlayerPrefs.Save();

    }

    private List<AttackType> GetAllAttackTypesListFromPlayerPrefs()
    {
        List<AttackType> returnAttackTypes = new();

        foreach (AttackType value in System.Enum.GetValues(typeof(AttackType)))
        {
            returnAttackTypes.Add(value);
        }

        return returnAttackTypes;
    }

    private List<AttackType> GetUnlockedAttackTypesListFromPlayerPrefs()
    {
        List<AttackType> returnAttackTypes = new();

        foreach (AttackType value in System.Enum.GetValues(typeof(AttackType)))
        {
            if (PlayerPrefs.GetInt("AttackType_" + value.ToString()) == 1)
            {
                returnAttackTypes.Add(value);
            }
        }

        return returnAttackTypes;
    }

    #region Global Access Functions

    public bool IsAttackTypeUnlocked(AttackType type)
    {
        return _playerDataContainer.AllAttackTypes.Contains(type);
    }

    public void UnlockAttackType(string attackTypeName)
    {
        int UnlockedCount = PlayerPrefs.GetInt("UnlockedAttackList_Count");

        for (int keyIndex = 0; keyIndex < UnlockedCount; keyIndex++)
        {
            string keyName = PlayerPrefs.GetString("UnlockedAttackList_" + keyIndex);

            if (System.Enum.TryParse(keyName, out AttackType type))
            {
            }
        }
    }


    #endregion
}