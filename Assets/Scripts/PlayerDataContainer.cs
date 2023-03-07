using System.Collections.Generic;

[System.Serializable]
public class PlayerDataContainer
{
    public List<PlayerAttacksData> AllAttackTypesData = new();
    public int coinsAmount = 0;
    public int totalLevels = 0;
    public int unlockedLevelsCount = 0;
    public int selectedLevelIndex = 0;
}
public class PlayerAttacksData
{
    public AttackType AttackType;
    public int isUnlocked = 0;
}