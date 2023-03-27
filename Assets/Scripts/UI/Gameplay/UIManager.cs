using DG.Tweening;
using GameAnalyticsSDK;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GAME_MODE INFO UI")] [Header("Rect Components")]
    public GameObject rootCanvas;

    public GameObject incomingWavePanel;
    public GameObject pausePanel;
    public GameObject floatingTextPanel;
    public GameObject unitUpgradesPanel;
    public GameObject headerPanel;

    [Header("Image Components")] public Image resourceMeter;

    [Header("Text Components")] public TMP_Text resourcesCount;
    public TMP_Text m_warningText;
    public TMP_Text nextWaveTimer;


    [Header("Animator Components")] public Animator resourceMeterAnimator;

    [Header("GLOBAL REFRENCE UI")] public TMP_Text m_damageTextPrefab;
    public TMP_Text m_floatingTextPrefab;

    [Header("ENEMY DATA PARAMETERS")] public EnemySpawnMarker[] enemySpawnMarkers;
    public GameFinishView gameFinishView;


    private readonly Queue<TMP_Text> _damageTextQueue = new();
    private LevelLoader _levelLoader;

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
        rootCanvas = this.gameObject;
    }

    public virtual void Start()
    {
        _levelLoader = LevelLoader.Instance;

        SpawnDamageTexts();
    }

    private void SpawnDamageTexts()
    {
        for (var i = 0; i < 40; i++)
        {
            TMP_Text damageText = Instantiate(m_damageTextPrefab, rootCanvas.transform.position, Quaternion.identity, floatingTextPanel.transform);
            _damageTextQueue.Enqueue(damageText);
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

    // ReSharper disable Unity.PerformanceAnalysis
    public void MatchFinished(bool hasWon)
    {
        gameFinishView.StartGameFinishSequence(hasWon);
    }

    #region UI VISUAL EFFECTS

    public void ShowFloatingScore(float damageAmount, Vector3 atPosition, Color textColor)
    {
        if (!m_damageTextPrefab) return;
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(atPosition);

        if (_damageTextQueue.Count < 1) return;
        TMP_Text tempTxt = _damageTextQueue.Dequeue()
            ;
        tempTxt.transform.position = spawnPos;
        tempTxt.color = textColor;
        tempTxt.text = "+" + damageAmount;

        tempTxt.gameObject.SetActive(true);
        (tempTxt.transform as RectTransform).DOJump(spawnPos + new Vector3(0, 200, 0), 10, 2, 1).OnComplete(() =>
        {
            tempTxt.gameObject.SetActive(false);
            _damageTextQueue.Enqueue(tempTxt);
        }).SetRecyclable(true);
        (tempTxt.transform as RectTransform).DOMoveX(spawnPos.x + Random.Range(-100, 100), 1).SetRecyclable(true);
    }

    public void ShowFloatingResourceRemovedUI(string text, Vector3 atPosition, Color textColor)
    {
        if (!m_floatingTextPrefab) return;
        var spawnPos = Camera.main.WorldToScreenPoint(atPosition);


        var tempTxt = Instantiate(m_floatingTextPrefab, spawnPos, Quaternion.identity, rootCanvas.transform);

        tempTxt.transform.position = spawnPos;
        tempTxt.color = textColor;
        tempTxt.text = text;

        tempTxt.gameObject.SetActive(true);
        (tempTxt.transform as RectTransform).DOJump(spawnPos + new Vector3(0, 100, 0), 10, 1, 1.5f).OnComplete(() => { tempTxt.gameObject.SetActive(false); });
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ShowNewWaveInfo(WaveSpawnData waveData)
    {
        if (incomingWavePanel.TryGetComponent(out CanvasGroup wavePanelCanvasGroup))
        {
            headerPanel.TryGetComponent(out CanvasGroup headerCanvasGroup);
            if (headerCanvasGroup)
                headerCanvasGroup.DOFade(0, 0.5f);

            wavePanelCanvasGroup.alpha = 0;
            incomingWavePanel.SetActive(true);
            var mySequence = DOTween.Sequence().SetRecyclable(true);
            mySequence.Append(wavePanelCanvasGroup.DOFade(1, 0.5f));
            mySequence.Append(wavePanelCanvasGroup.DOFade(0, 0.5f));
            mySequence.SetLoops(3);

            mySequence.OnComplete(() =>
            {
                wavePanelCanvasGroup.alpha = 0;
                incomingWavePanel.SetActive(false);

                if (headerCanvasGroup)
                    headerCanvasGroup.DOFade(1, 0.5f);
            });
        }

        foreach (var enemyData in waveData.enemySpawnData)
        {
            foreach (var marker in enemySpawnMarkers)
            {
                if (marker == null || marker.enemyType != enemyData.enemyType)
                {
                    continue;
                }

                var m = Instantiate(marker, incomingWavePanel.transform);
                m.ShowMarker(LevelManager.Instance.GetSpawnTransform(enemyData.spawnLocation), enemyData.enemyCount);
            }
        }
    }

    #endregion

    #region BUTTON_EVENTS

    public void EnablePausePanel()
    {
        if (!pausePanel)
        {
            Debug.LogWarning("UnlocksPanel is not assigned at: " + this);
            return;
        }

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
        if (!pausePanel)
        {
            Debug.LogWarning("UnlocksPanel is not assigned at: " + this);
            return;
        }

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
        _levelLoader.LoadScene(0);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    #endregion
}