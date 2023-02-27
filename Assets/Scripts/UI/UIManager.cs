using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

public class UIManager : Singleton<UIManager>
{

    [Header("GAMEMODE INFO UI")]
    [Header("Rect Componenets")]
    public Canvas rootcanvas;
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
    public Image unitSelectionCooldownTimerImage;
    public Image loadingfiller;

    [Header("Text Componenets")]
    public TMP_Text waveTxt;
    public TMP_Text overTxt;
    public TMP_Text enemiesCountTxt;
    public TMP_Text m_warningText;

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
            (m_warningText.transform as RectTransform).DOShakeAnchorPos(3, 25).OnComplete(() =>
            {
                m_warningText.text = "";
                m_warningText.gameObject.SetActive(false);
            });

        }
    }
    protected override void Awake()
    {
        base.Awake();
        // Instance = this;
        pauseBtn.onClick.AddListener(PauseButton);
    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(PauseButton);
    }
    internal void ShowText(string tempTxt)
    {
        ShowResponseMessage(tempTxt, waveTxt);
    }
    public virtual void Start()
    {
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
    public void CloseTheLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }

    internal void ShowResponseMessage(string message, TMP_Text waveTxtTemp)
    {
        waveTxtTemp.text = message;
        Sequence seq = DOTween.Sequence();
        waveTxtTemp.transform.gameObject.SetActive(true);
        seq.AppendInterval(2f);
        seq.AppendCallback(() => { waveTxtTemp.transform.gameObject.SetActive(false); });
    }

    public void PauseButton()
    {
        floatingTextPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
        pausePanel.transform.GetChild(1).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        pausePanel.transform.GetChild(1).DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).OnComplete(() =>
        {
            pausePanel.transform.GetChild(1).DOScale(new Vector3(1f, 1f, 1f), 0.15f).OnComplete(() => { Time.timeScale = 0; });
        });
    }
    public void ResumeButton(Button btn)
    {
        Time.timeScale = 1;
        btn.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.3f).OnComplete(() =>
        {
            pausePanel.SetActive(false);
            floatingTextPanel.SetActive(true);
            btn.transform.localScale = Vector3.one;
        });

    }
    public void RestartButton(Button btn)
    {
        Time.timeScale = 1;
        btn.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.3f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            btn.transform.localScale = Vector3.one;
        });

    }
    public void QuitButton(Button btn)
    {
        Time.timeScale = 1;
        btn.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.3f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync(0);
            btn.transform.localScale = Vector3.one;
        });

    }
    public void TempVoid()
    {
        GameOverVoid(true);
    }
    public void GameOverVoid(bool hasWon)
    {
        floatingTextPanel.gameObject.SetActive(false);

        int levelNum = PlayerPrefs.GetInt("CurrentLevel");
        string eventName = "Level_0" + (levelNum + 1);

        if (hasWon && (PlayerPrefs.GetInt("CurrentLevel") < 5))
        {
            gameWinPanel.SetActive(true);
            int tempInt = PlayerPrefs.GetInt("CurrentLevel");
            tempInt++;
            PlayerPrefs.SetInt("CurrentLevel", tempInt);
            PlayerDataManager.Instance.CoinsAmount += 1;
        }
        else
        {
            gameLostPanel.SetActive(true);
        }
        if (hasWon)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, eventName);
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, eventName);
        }

        Time.timeScale = 0;
    }

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
}
