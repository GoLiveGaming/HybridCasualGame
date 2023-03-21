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
public enum TowerState
{
    Idle,
    Attack,
    Destroyed
}

public enum EnemyTypes
{
    Grunt,
    Heavy,
    Archer,
    Elite
}


[Serializable]
public class PlayerUnit
{
    public AttackType unitType;
    public PlayerTower unitPrefab;
}

[Serializable]
public class MergingCombinations
{
    public AttackType combinesWith;
    public AttackType toYield;
}


