using System;
using UnityEngine;

public class Utils : MonoBehaviour
{

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
    FloodAttack

}
public enum TowerState
{
    Idle,
    Attack,
    Destroyed
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
