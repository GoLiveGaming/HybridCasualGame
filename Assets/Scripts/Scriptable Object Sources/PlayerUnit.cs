using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerUnit", menuName = "ScriptableObjects/PlayerUnit", order = 1)]
public class PlayerUnit : ScriptableObject
{

    [Space(10), Header("PLAYER TOWER PROPERTIES"), Space(10)]

    public AttackType unitType;
    [Range(0, 10)]
    public int resourceCost = 2;
    public PlayerTower unitPrefab;

    [Space(2), Header("Merging Properties")]

    public bool supportsCombining = false;
    [ShowIf("supportsCombining")] public List<MergingCombinations> possibleCombinations = new();
    //ONLY FOR UPGRADED UNITS
    [HideIf("supportsCombining")] public Sprite sourceComponent01;
    [HideIf("supportsCombining")] public Sprite sourceComponent02;

    [Header("UI COMPONENTS")]
    public Sprite indicatorColorIcon;  //Used to indicate color in PlayerTowerUI
    public Sprite deployButtonSprite;
    public Sprite statsUISprite;
}
