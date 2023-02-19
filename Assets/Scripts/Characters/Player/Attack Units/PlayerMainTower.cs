public class PlayerMainTower : PlayerUnitBase
{
    protected override void Start()
    {
        base.Start();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Add(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Add(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (mainPlayerControl && mainPlayerControl.mainPlayerTower.Contains(this))
            mainPlayerControl.mainPlayerTower.Remove(this);
    }
}
