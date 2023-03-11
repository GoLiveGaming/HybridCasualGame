using DG.Tweening;
using GameAnalyticsSDK;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    [Header("GAMEMODE INFO UI")]
    [Header("Rect Componenets")]
    public GameObject rootcanvas;
    public GameObject gameplayItemsCanvas;
    public GameObject unitSelectionCanvas;
    public GameObject pausePanel;
    public GameObject gameWinPanel;
    public GameObject gameLostPanel;
    public GameObject loadingPanel;
    public GameObject floatingTextPanel;

    [Header("Button Componenets")]
    public Button pauseBtn;
    public Button resumeBtn;
    public Button restartBtn;
    public Button quitBtn;

    [Header("Image Componenets")]
    public Image resourceMeter;
    public Image loadingfiller;

    [Header("Text Componenets")]
    public TMP_Text waveTxt;
    public TMP_Text enemiesCountTxt;
    public TMP_Text m_warningText;
    public TMP_Text resourcesCount;
    public TMP_Text meleeCount;
    public TMP_Text heaviesCount;
    public TMP_Text rangedCount;
    public TMP_Text eliteCount;
    public TMP_Text totalEnemiesCount;
    public TMP_Text lastwaveInLosePanel;
    public TMP_Text lastwaveInWinPanel;
    public TMP_Text enemiesSlainInLosePanel;
    public TMP_Text enemiesSlainInWinPanel;

    [Header("Animator Components")]
    public Animator resourceMeterAnimator;
    public Animator waveAnimator;

    [Header("GLOBAL REFRENCE UI")]
    public TMP_Text m_damageTextPrefab;
    public TMP_Text m_floatingTextPrefab;
    readonly Queue<TMP_Text> damageTextQueue = new();
    private LevelLoader levelLoader;
    private PlayerDataManager playerDataManager;

    public string ShowWarningText
    {
        get { return m_warningText.text; }
        set
        {
            if (!m_warningText) return;
            m_warningText.text = value;
            m_warningText.gameObject.SetActive(true);
            (m_warningText.transform as RectTransform).DOShakeAnchorPos(3, 15).OnComplete(() =>
            {
                m_warningText.text = "";
                m_warningText.gameObject.SetActive(false);
            });

        }
    }
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1.0f;
        rootcanvas = this.gameObject;
    }
    public virtual void Start()
    {
        levelLoader = LevelLoader.Instance;
        playerDataManager = PlayerDataManager.Instance;
        SpawndamageTexts();
    }
    void SpawndamageTexts()
    {
        for (int i = 0; i < 40; i++)
        {
            TMP_Text damageText = Instantiate(m_damageTextPrefab, rootcanvas.transform.position, Quaternion.identity, floatingTextPanel.transform);
            damageTextQueue.Enqueue(damageText);
            damageText.gameObject.SetActive(false);
        }
    }

    internal void ShowResponseMessage(string message, TMP_Text waveTxtTemp)
    {
        waveTxtTemp.text = message;
        Sequence seq = DOTween.Sequence();
        waveTxtTemp.transform.gameObject.SetActive(true);
        seq.AppendInterval(2f);
        seq.AppendCallback(() => { waveTxtTemp.transform.gameObject.SetActive(false); });
    }


    [ContextMenu("Win Level)")]
    public void WinMatch()
    {
        MatchFinished(true);
    }
    [ContextMenu("Loose Level)")]
    public void LooseMatch()
    {
        MatchFinished(false);
    }
    public void MatchFinished(bool hasWon)
    {
        floatingTextPanel.gameObject.SetActive(false);

        string eventName = "Level_0" + (playerDataManager.SelectedLevelIndex);

        if (hasWon)
        {
            gameWinPanel.SetActive(true);
            lastwaveInWinPanel.text = LevelManager.Instance.levelData[LevelManager.Instance.levelNum].Waves[LevelManager.Instance.WaveIndexMain].waveNum.ToString();
            enemiesSlainInWinPanel.text = (LevelManager.Instance.maxEnemyCount - LevelManager.Instance.deadEnemiesCount).ToString();
            playerDataManager.UnlockedLevelsCount += 1;
            playerDataManager.CoinsAmount += 1;

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, eventName);
        }
        else
        {
            gameLostPanel.SetActive(true);
            lastwaveInLosePanel.text = LevelManager.Instance.levelData[LevelManager.Instance.levelNum].Waves[LevelManager.Instance.WaveIndexMain].waveNum.ToString();
            enemiesSlainInLosePanel.text = (LevelManager.Instance.maxEnemyCount - LevelManager.Instance.deadEnemiesCount).ToString();

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, eventName);
        }


        if (gameplayItemsCanvas && gameplayItemsCanvas.TryGetComponent(out CanvasGroup group))
        {
            group.DOFade(0, 0.25f).OnComplete(() => Time.timeScale = 0);
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    #region UI VISUAL EFFECTS
    public void ShowFloatingDamage(float damageAmount, Vector3 atPosition, Color textColor)
    {
        if (!m_damageTextPrefab) return;
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(atPosition);

        if (damageTextQueue.Count < 1) return;
        //  TMP_Text damageText = Instantiate(m_damageTextPrefab, spawnPos, Quaternion.identity, rootcanvas.transform);
        TMP_Text tempTxt = damageTextQueue.Dequeue();
        tempTxt.transform.position = spawnPos;
        tempTxt.color = textColor;
        tempTxt.text = damageAmount.ToString();
        tempTxt.gameObject.SetActive(true);
        (tempTxt.transform as RectTransform).DOJump(spawnPos + new Vector3(0, 200, 0), 10, 2, 1).OnComplete(() =>
        {
            tempTxt.gameObject.SetActive(false);
            damageTextQueue.Enqueue(tempTxt);
        });
        (tempTxt.transform as RectTransform).DOMoveX(spawnPos.x + Random.Range(-100, 100), 1);

    }
    public void ShowFloatingText(string text, Vector3 atPosition, Color textColor)
    {
        if (!m_floatingTextPrefab) return;
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(atPosition);


        TMP_Text tempTxt = Instantiate(m_floatingTextPrefab, spawnPos, Quaternion.identity, rootcanvas.transform);

        tempTxt.transform.position = spawnPos;
        tempTxt.color = textColor;
        tempTxt.text = text;
        tempTxt.gameObject.SetActive(true);
        (tempTxt.transform as RectTransform).DOJump((spawnPos + new Vector3(0, 100, 0)), 10, 1, 2).OnComplete(() =>
        {
            tempTxt.gameObject.SetActive(false);
        });

    }
    public string FormatStringNextLineOnUpperCase(string value)
    {

        string formattedString = "";

        for (int i = 0; i < value.Length; i++)
        {
            if (i > 0 && char.IsUpper(value[i]))
            {
                formattedString += "\n";
            }
            formattedString += value[i];
        }

        return formattedString;
    }

    internal void WaveBouncyText(WaveData firstWave, WaveData secondWave)
    {
        int melee = 0, heavies = 0, ranged = 0, elite = 0;
        totalEnemiesCount.text = firstWave.totalEniemies.ToString();
        for (int i = 0; i < firstWave.enemyData.Length; i++)
        {
            if (firstWave.enemyData[i].enemyType == EnemyTypes.Melee) { melee += firstWave.enemyData[i].enemyCount; meleeCount.text = melee.ToString(); }
            else if (firstWave.enemyData[i].enemyType == EnemyTypes.Heavies) { heavies += firstWave.enemyData[i].enemyCount; heaviesCount.text = heavies.ToString(); }
            else if (firstWave.enemyData[i].enemyType == EnemyTypes.Ranged) { ranged += firstWave.enemyData[i].enemyCount; rangedCount.text = ranged.ToString(); }
            else if (firstWave.enemyData[i].enemyType == EnemyTypes.RangedBig) { elite += firstWave.enemyData[i].enemyCount; eliteCount.text = elite.ToString(); }
        }
        waveTxt.text = "Wave " + firstWave.waveNum + " Incoming";
        if (waveAnimator) waveAnimator.gameObject.SetActive(true);
        if (waveAnimator) waveAnimator.SetBool("EnableWave", true);
        waveTxt.transform.DOScale(new Vector3(1f, 1f, 1f), 2f).OnComplete(() =>
        {
            if (waveAnimator) waveAnimator.SetBool("EnableWave", false);
            if (waveAnimator) waveAnimator.gameObject.SetActive(false);

            if (secondWave != null)
            {
                melee = 0; heavies = 0; ranged = 0; elite = 0;
                meleeCount.text = 0.ToString(); heaviesCount.text = 0.ToString(); rangedCount.text = 0.ToString(); eliteCount.text = 0.ToString();
                for (int i = 0; i < secondWave.enemyData.Length; i++)
                {
                    if (secondWave.enemyData[i].enemyType == EnemyTypes.Melee) { melee += secondWave.enemyData[i].enemyCount; meleeCount.text = melee.ToString(); }
                    else if (secondWave.enemyData[i].enemyType == EnemyTypes.Heavies) { heavies += secondWave.enemyData[i].enemyCount; heaviesCount.text = heavies.ToString(); }
                    else if (secondWave.enemyData[i].enemyType == EnemyTypes.Ranged) { ranged += secondWave.enemyData[i].enemyCount; rangedCount.text = ranged.ToString(); }
                    else if (secondWave.enemyData[i].enemyType == EnemyTypes.RangedBig) { elite += secondWave.enemyData[i].enemyCount; eliteCount.text = elite.ToString(); }
                }
                totalEnemiesCount.text = secondWave.totalEniemies.ToString();
                // waveTxt.text = "Wave " + secondWave.waveNum + "\n" + secondWave.totalEniemies + " Enemies Incoming";
            }
            else
            {
                totalEnemiesCount.text = 0.ToString(); meleeCount.text = 0.ToString(); heaviesCount.text = 0.ToString(); rangedCount.text = 0.ToString(); eliteCount.text = 0.ToString();
            }
        });
    }
    #endregion

    #region BUTTON_EVENTS

    public void EnablePausePanel()
    {
        if (!pausePanel) { Debug.LogWarning("UnlocksPanel is not assigned at: " + this); return; }

        if (pausePanel.TryGetComponent(out EnhancedPanels panel))
        {
            panel.EnablePanel();
            panel.OnPanelActivation.AddListener(() => Time.timeScale = 0);
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
    public void DisablePausePanel()
    {
        if (!pausePanel) { Debug.LogWarning("UnlocksPanel is not assigned at: " + this); return; }

        if (pausePanel.TryGetComponent(out EnhancedPanels panel))
        {
            panel.DisablePanel();

            panel.OnPanelActivation.AddListener(() => Time.timeScale = 1);
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }

    }
    public void RestartButton()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitButton()
    {
        levelLoader.LoadScene(0);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }
    #endregion
}
