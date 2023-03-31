using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUnit", menuName = "ScriptableObjects/PlayerUnit", order = 1)]
public class PlayerUnit : ScriptableObject
{
    [Space(10)]
    public AttackType unitType;
    public PlayerTower unitPrefab;
    public int resourcesCost;

    [Header("UI COMPONENTS")]
    public Sprite indicatorColorIcon;  //Used to indicate color in PlayerTowerUI
    public Sprite deployButtonSprite;
    public Sprite statsUISprite;
}
