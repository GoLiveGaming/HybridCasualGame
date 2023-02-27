using System.Collections.Generic;

public class PlayerDataContainer
{
    public List<PlayerAttacksData> AllAttackTypesData = new();
    public int coinsAmount = 0;
}
public class PlayerAttacksData
{
    public AttackType AttackType;
    public int isUnlocked = 0;
}