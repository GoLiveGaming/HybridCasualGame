using NaughtyAttributes;
using UnityEngine;

public class PlayerTowerMain : PlayerUnitBase
{
    [Space(2), Header("PLAYER TOWER PROPERTIES"), Space(2)]
    [ReadOnly] public PlayerTowerMainUI playerTowerUI;

    private UIManager _uiManager;
    protected void Start()
    {
        AddUnitToMain();
        if (_mainPlayerControl && !_mainPlayerControl._mainPlayerTower)
            _mainPlayerControl._mainPlayerTower = this;

        _uiManager = UIManager.Instance;


        playerTowerUI = Instantiate(_mainPlayerControl.playerTowerMainUIPrefab, _uiManager.rootCanvas.transform);
        playerTowerUI.InitializeUI(this);


        GetComponent<Stats>().InitializeStats();

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
