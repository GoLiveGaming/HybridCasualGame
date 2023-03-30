using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Stats))]
public class PlayerUnitBase : MonoBehaviour
{
    [ReadOnly] internal MainPlayerControl _mainPlayerControl;
    protected void AddUnitToMain()
    {

        if (!_mainPlayerControl) _mainPlayerControl = MainPlayerControl.Instance;
        if (_mainPlayerControl && !_mainPlayerControl.activePlayerTowersList.Contains(this))
            _mainPlayerControl.activePlayerTowersList.Add(this);

    }

    protected void RemoveUnitFromMain()
    {
        if (!_mainPlayerControl) _mainPlayerControl = MainPlayerControl.Instance;
        if (_mainPlayerControl && _mainPlayerControl.activePlayerTowersList.Contains(this))
            _mainPlayerControl.activePlayerTowersList.Remove(this);
    }
}
