using System;
using UnityEngine;

public class PlayerDataManager : SingletonPersistent<PlayerDataManager>
{

    [SerializeField] private PlayerDataContainer _playerData = new();
    public PlayerDataContainer PlayerData { get { return _playerData; } }


    protected override void Awake()
    {
        base.Awake();

        // Set Default Value for all items if playing for first time
        if (!PlayerPrefs.HasKey("Initialized") || PlayerPrefs.GetInt("Initialized") == 0)
        {
            foreach (AttackType value in Enum.GetValues(typeof(AttackType)))
            {
                PlayerPrefs.SetInt("AttackType_" + value.ToString(), 0);
            }
            //Unlocking some attacks by default
            PlayerPrefs.SetInt("AttackType_" + AttackType.FireAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.WindAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.WaterAttack.ToString(), 1);
            PlayerPrefs.SetInt("AttackType_" + AttackType.LightningAttack.ToString(), 1);


            PlayerPrefs.SetInt("coinsAmount", 0);
            PlayerPrefs.SetInt("unlockedLevelsCount", 1);
            PlayerPrefs.SetInt("selectedLevelIndex", 1);
            PlayerPrefs.SetInt("totalLevels", 5);


            PlayerPrefs.SetInt("AttackTypesList_Count", _playerData.AllAttackTypesData.Count);

            PlayerPrefs.SetInt("Initialized", 1);
        }
        UpdateDataFromPlayerPrefs();
        PlayerPrefs.Save();
    }

    [ContextMenu("UpdateDataFromPlayerPrefs")]
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

        _playerData.coinsAmount = PlayerPrefs.GetInt("coinsAmount");
        _playerData.unlockedLevelsCount = PlayerPrefs.GetInt("unlockedLevelsCount");
        _playerData.selectedLevelIndex = PlayerPrefs.GetInt("selectedLevelIndex");
        _playerData.totalLevels = PlayerPrefs.GetInt("totalLevels");

    }
    [ContextMenu("SaveDataToPlayerPrefs")]
    private void SaveDataToPlayerPrefs()
    {
        //Update Attack Types in Playerprefs
        foreach (PlayerAttacksData data in _playerData.AllAttackTypesData)
        {
            if (data.isUnlocked == 1)
            {
                PlayerPrefs.SetInt("AttackType_" + data.AttackType.ToString(), 1);
            }
        }

        //Update Coins Amount in PlayerPrefs
        PlayerPrefs.SetInt("coinsAmount", _playerData.coinsAmount);
        PlayerPrefs.SetInt("unlockedLevelsCount", _playerData.unlockedLevelsCount);
        PlayerPrefs.SetInt("selectedLevelIndex", _playerData.selectedLevelIndex);
        PlayerPrefs.SetInt("totalLevels", _playerData.totalLevels);


        PlayerPrefs.Save();
    }

    #region Global Access Functions

    #region Data getters & setters
    public int CoinsAmount
    {
        get { return _playerData.coinsAmount; }
        set
        {
            _playerData.coinsAmount = value;
            SaveDataToPlayerPrefs();
        }
    }
    public int UnlockedLevelsCount
    {
        get { return _playerData.unlockedLevelsCount; }
        set
        {
            int lvl = value;

            lvl = Mathf.Clamp(lvl, 0, _playerData.totalLevels);
            _playerData.unlockedLevelsCount = lvl;
            _playerData.selectedLevelIndex = lvl;

            SaveDataToPlayerPrefs();
        }
    }

    public int SelectedLevelIndex
    {
        get { return _playerData.selectedLevelIndex; }
        set { _playerData.selectedLevelIndex = value; }
    }
    public int TotalLevels { get { return _playerData.totalLevels; } }



    #endregion
    public bool UnlockAttackType(AttackType attackType)
    {
        if (CoinsAmount <= 0) return false;
        if (IsAttackTypeUnlocked(attackType)) return false;

        foreach (var data in _playerData.AllAttackTypesData)
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
        foreach (PlayerAttacksData data in _playerData.AllAttackTypesData)
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
        UpdateDataFromPlayerPrefs();
    }

    #endregion
}