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

    [Header("Animator Components")]
    public Animator resourceMeterAnimator;

    [Header("GLOBAL REFRENCE UI")]
    public TMP_Text m_damageTextPrefab;
    readonly Queue<TMP_Text> damageTextQueue = new();

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
        if (resourceMeter) resourceMeterAnimator = resourceMeter.GetComponent<Animator>();
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


    public void MatchFinished(bool hasWon)
    {
        floatingTextPanel.gameObject.SetActive(false);

        int levelNum = PlayerPrefs.GetInt("CurrentLevel");
        string eventName = "Level_0" + (levelNum + 1);

        if (hasWon && (PlayerPrefs.GetInt("CurrentLevel") < 5))
        {
            gameWinPanel.SetActive(true);

            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
            PlayerDataManager.Instance.CoinsAmount += 1;

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, eventName);
        }
        else
        {
            gameLostPanel.SetActive(true);

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, eventName);
        }


        if (gameplayItemsCanvas && gameplayItemsCanvas.TryGetComponent(out CanvasGroup group))
        {
            group.DOFade(0, 2).OnComplete(() => Time.timeScale = 0);
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    #region UI VISUAL EFFECTS
    public void ShowFloatingDamage(float damageAmount, Vector3 atPosition, Color textColor)
    {

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
        (tempTxt.transform as RectTransform).DOLocalMoveX(Random.Range(-200, 200), 1);

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

    internal void WaveBouncyText(int firstLevel, int firstEnemies, int secondLevel, int secondEnemies)
    {
        waveTxt.text = "Wave " + firstLevel + "\n" + firstEnemies + " Enemies";
        waveTxt.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 2f).OnComplete(() =>
        {
            waveTxt.transform.localScale = Vector3.one;
            if (secondEnemies > 0)
                waveTxt.text = "Wave " + secondLevel + "\n" + secondEnemies + " Enemies Incoming";
            else
                waveTxt.text = "";
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
        Time.timeScale = 1;

        SceneManager.LoadSceneAsync(0);
    }
    #endregion
}
