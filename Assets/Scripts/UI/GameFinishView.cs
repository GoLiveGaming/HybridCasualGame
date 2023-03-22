using GameAnalyticsSDK;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishView : MonoBehaviour
{
    [SerializeField] private Sprite wonPanelBG;
    [SerializeField] private Sprite lostPanelBG;
    [SerializeField] private Image panelBG;
    [SerializeField] private GameObject wonHeader;
    [SerializeField] private GameObject lostHeader;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI enemyWavesCompletedNum;


    private MainPlayerControl mainPlayerControl;
    private PlayerDataManager playerDataManager;
    private UIManager uiManager;
    bool initialized = false;

    void Initialize()
    {
        mainPlayerControl = MainPlayerControl.Instance;
        playerDataManager = PlayerDataManager.Instance;
        uiManager = UIManager.Instance;
        gameObject.SetActive(false);
        initialized = true;
    }

    public void StartGameFinishSequence(bool hasWon)
    {
        if (!initialized) Initialize();

        totalScoreText.text = mainPlayerControl.CalculateTotalScore().ToString();
        enemiesKilledText.text = mainPlayerControl.TotalEnemiesKilledNum.ToString();
        enemyWavesCompletedNum.text = mainPlayerControl.EnemyWavesCompletedNum.ToString();

        uiManager.floatingTextPanel.SetActive(false);
        string eventName = "Level_0" + (playerDataManager.SelectedLevelIndex);

        if (hasWon)
        {
            wonHeader.SetActive(true);
            lostHeader.SetActive(false);
            panelBG.sprite = wonPanelBG;
            playerDataManager.UnlockedLevelsCount += 1;
            playerDataManager.CoinsAmount += 1;

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, eventName);
        }
        else
        {
            wonHeader.SetActive(false);
            lostHeader.SetActive(true);
            panelBG.sprite = lostPanelBG;

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, eventName);
        }
        gameObject.SetActive(true);

        Time.timeScale = 0;
    }

}
