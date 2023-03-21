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
    public GameObject incomingWavePanel;
    public GameObject pausePanel;
    public GameObject loadingPanel;
    public GameObject floatingTextPanel;
    public GameObject unitUpgradesPanel;

    [Header("Image Componenets")]
    public Image resourceMeter;
    public Image loadingfiller;

    [Header("Text Componenets")]
    public TMP_Text resourcesCount;
    public TMP_Text m_warningText;
    public TMP_Text nextWaveTimer;


    [Header("Animator Components")]
    public Animator resourceMeterAnimator;

    [Header("GLOBAL REFRENCE UI")]
    public TMP_Text m_damageTextPrefab;
    public TMP_Text m_floatingTextPrefab;

    [Header("ENEMY DATA PARAMETERS")]
    public EnemySpawnMarker[] enemySpawnMarkers;
    public GameFinishView gameFinishView;




    private readonly Queue<TMP_Text> damageTextQueue = new();
    private LevelLoader levelLoader;

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
        gameFinishView.StartGameFinishSequence(hasWon);
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

    internal void ShowNewWaveInfo(WaveData waveData)
    {

        if (incomingWavePanel.TryGetComponent(out CanvasGroup wavePanelCanvasGroup))
        {
            incomingWavePanel.SetActive(true);
            Sequence mySequence = DOTween.Sequence().SetRecyclable(true);
            mySequence.Append(wavePanelCanvasGroup.DOFade(0, 0.5f));
            mySequence.Append(wavePanelCanvasGroup.DOFade(1, 0.5f));
            mySequence.SetLoops(3);

            mySequence.OnComplete(() =>
            {
                incomingWavePanel.SetActive(false);
            });

        }

        foreach (EnemyData enemyData in waveData.enemyData)
        {
            foreach (EnemySpawnMarker marker in enemySpawnMarkers)
            {
                if (marker != null && marker.enemyType == enemyData.enemyType)
                {
                    EnemySpawnMarker m = Instantiate(marker, rootcanvas.transform);
                    m.ShowMarker(LevelManager.Instance.GetSpawnTransform(enemyData.spawnLocation), enemyData.enemyCount);
                }
            }
        }

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
