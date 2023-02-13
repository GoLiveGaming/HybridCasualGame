
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class PlayerUnitBase : MonoBehaviour
{
    [ReadOnly] public MainPlayerControl mainPlayerControl;

    protected virtual void Awake() { }

    protected virtual void Start()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        if (!mainPlayerControl.activePlayerTowersList.Contains(this))
            mainPlayerControl.activePlayerTowersList.Add(this);
    }

    protected virtual void OnEnable()
    {
        if (mainPlayerControl)
            mainPlayerControl.activePlayerTowersList.Add(this);
    }
    protected virtual void OnDisable()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        mainPlayerControl.activePlayerTowersList.Remove(this);
    }
}
