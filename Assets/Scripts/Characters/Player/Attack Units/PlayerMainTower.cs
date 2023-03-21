public class PlayerMainTower : PlayerUnitBase
{
    protected void Start()
    {
        AddUnitToMain();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower)
            mainPlayerControl.mainPlayerTower = this;
    }

    protected void OnEnable()
    {
        AddUnitToMain();
        if (mainPlayerControl && !mainPlayerControl.mainPlayerTower)
            mainPlayerControl.mainPlayerTower = this;
    }
    protected void OnDisable()
    {
        RemoveUnitFromMain();
        if (mainPlayerControl && mainPlayerControl.mainPlayerTower)
            mainPlayerControl.mainPlayerTower = null;
    }
}
