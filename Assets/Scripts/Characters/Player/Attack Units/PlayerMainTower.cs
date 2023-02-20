public class PlayerMainTower : PlayerUnitBase
{
    protected void Start()
    {
        AddUnitToMain();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Add(this);
    }

    protected void OnEnable()
    {
        AddUnitToMain();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Add(this);
    }
    protected void OnDisable()
    {
        RemoveUnitFromMain();
        if (mainPlayerControl && mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Remove(this);
    }
}
