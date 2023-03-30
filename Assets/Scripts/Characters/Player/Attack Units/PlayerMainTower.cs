public class PlayerMainTower : PlayerUnitBase
{
    protected void Start()
    {
        AddUnitToMain();
        if (_mainPlayerControl && !_mainPlayerControl._mainPlayerTower)
            _mainPlayerControl._mainPlayerTower = this;
    }

    protected void OnEnable()
    {
        AddUnitToMain();
        if (_mainPlayerControl && !_mainPlayerControl._mainPlayerTower)
            _mainPlayerControl._mainPlayerTower = this;
    }
    protected void OnDisable()
    {
        RemoveUnitFromMain();
        if (_mainPlayerControl && _mainPlayerControl._mainPlayerTower)
            _mainPlayerControl._mainPlayerTower = null;
    }
}
