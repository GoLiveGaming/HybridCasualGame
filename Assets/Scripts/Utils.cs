using System;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Instance;
    public static bool isGamePaused = false;

    private void Awake()
    {
        Instance = this;
    }



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
    FloodAttack,
    NONE

}

public enum EnemyTypes
{
    Grunt,
    Heavy,
    Archer,
    Elite
}


//[Serializable]
//public class PlayerUnit
//{
//    [Space(10)]
//    public AttackType unitType;
//    public PlayerTower unitPrefab;
//    public int resourcesCost;

//    [Header("UI COMPONENTS")]
//    public Sprite indicatorColorIcon;  //Used to indicate color in PlayerTowerUI
//    public Sprite deployButtonSprite;
//    public Sprite statsUISprite;
//}

[Serializable]
public class MergingCombinations
{
    public AttackType combinesWith;
    public AttackType toYield;
}


