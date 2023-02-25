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
            PlayerPrefs.SetInt("Initialized", 1);
            _playerDataContainer.UnlockedAttackTypes.Add(AttackType.FireAttack);
            _playerDataContainer.UnlockedAttackTypes.Add(AttackType.WindAttack);
            _playerDataContainer.UnlockedAttackTypes.Add(AttackType.WaterAttack);
            _playerDataContainer.UnlockedAttackTypes.Add(AttackType.LightningAttack);

            PlayerPrefs.SetInt("UnlockedAttackList_Count", _playerDataContainer.UnlockedAttackTypes.Count);

            for (var i = 0; i < _playerDataContainer.UnlockedAttackTypes.Count; i++)
            {
                PlayerPrefs.SetString("UnlockedAttackList_" + i, _playerDataContainer.UnlockedAttackTypes[i].ToString());
            }

        }
        _playerDataContainer.UnlockedAttackTypes = GetUnlockedAttackTypesListFromPlayerPrefs();
        PlayerPrefs.Save();

    }

    private List<AttackType> GetUnlockedAttackTypesListFromPlayerPrefs()
    {
        List<AttackType> returnAttackTypes = new();
        int UnlockedCount = PlayerPrefs.GetInt("UnlockedAttackList_Count");

        for (int keyIndex = 0; keyIndex < UnlockedCount; keyIndex++)
        {
            string keyName = PlayerPrefs.GetString("UnlockedAttackList_" + keyIndex);

            if (System.Enum.TryParse(keyName, out AttackType type))
            {
                returnAttackTypes.Add(type);
            }
        }
        return returnAttackTypes;
    }



    #region Global Access Functions

    public bool IsAttackTypeUnlocked(AttackType type)
    {
        return _playerDataContainer.UnlockedAttackTypes.Contains(type);
    }


    #endregion
}