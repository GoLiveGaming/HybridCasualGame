using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GAME_MODE INFO UI")] [Header("Rect Components")]
    public GameObject rootCanvas;

    public GameObject pausePanel;
    public GameObject floatingTextPanel;
    public GameObject unitUpgradesPanel;
    public GameObject headerPanel;

    [Header("Canvas Groups")] public CanvasGroup wavePanelGroup;
    public CanvasGroup waveImageGroup;

    [Header("Image Components")] public Image resourceMeter;

    [Header("Text Components")] public TMP_Text resourcesCount;
    public TMP_Text warningText;
    public TMP_Text nextWaveTimer;
    public TMP_Text scoreText;

    [Header("Animator Components")] public Animator resourceMeterAnimator;

    [Header("GLOBAL REFERENCE UI")] public TMP_Text damageTextPrefab;
    public TMP_Text floatingTextPrefab;

    [Header("ENEMY DATA PARAMETERS")] public EnemySpawnMarker enemySpawnMarker;
    public GameFinishView gameFinishView;


    private readonly Queue<TMP_Text> _damageTextQueue = new();
    private LevelLoader _levelLoader;

    public string ShowWarningText
    {
        get { return warningText.text; }
        set
        {
            if (!warningText) return;
            warningText.text = value;
            warningText.gameObject.SetActive(true);
            (warningText.transform as RectTransform).DOShakeAnchorPos(3, 15).OnComplete(() =>
            {
                warningText.text = "";
                warningText.gameObject.SetActive(false);
            });
        }
    }

    private MainPlayerControl _mainPlayerControl;

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1.0f;
        rootCanvas = this.gameObject;
    }

    public virtual void Start()
    {
        _levelLoader = LevelLoader.Instance;
        _mainPlayerControl = MainPlayerControl.Instance;
        StartCoroutine(UpdateScoreText());
        SpawnDamageTexts();
    }

    private void SpawnDamageTexts()
    {
        for (var i = 0; i < 40; i++)
        {
            var damageText = Instantiate(damageTextPrefab, rootCanvas.transform.position, Quaternion.identity, floatingTextPanel.transform);
            _damageTextQueue.Enqueue(damageText);
            damageText.gameObject.SetActive(false);
        }
    }

    private IEnumerator UpdateScoreText()
    {
        while (true)
        {
            scoreText.text = _mainPlayerControl.CalculateTotalScore().ToString();
            yield return new WaitForSeconds(2);
        }
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
        if (!damageTextPrefab) return;
        if (Camera.main == null)
        {
            return;
        }

        var spawnPos = Camera.main.WorldToScreenPoint(atPosition);

        if (_damageTextQueue.Count < 1) return;
        var tempTxt = _damageTextQueue.Dequeue();
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

    public void ShowNotEnoughResourcesEffect()
    {
        if (!resourcesCount) return;
        (resourcesCount.transform as RectTransform).DOShakeAnchorPos(1, 10);
    }

    public void ShowFloatingResourceRemovedUI(string text, Vector3 atPosition, Color textColor)
    {
        if (!floatingTextPrefab) return;
        if (Camera.main == null)
        {
            return;
        }

        var spawnPos = Camera.main.WorldToScreenPoint(atPosition);


        var tempTxt = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity, rootCanvas.transform);

        tempTxt.transform.position = spawnPos;
        tempTxt.color = textColor;
        tempTxt.text = text;

        tempTxt.gameObject.SetActive(true);
        (tempTxt.transform as RectTransform).DOJump(spawnPos + new Vector3(0, 100, 0), 10, 1, 1.5f).OnComplete(() => { tempTxt.gameObject.SetActive(false); });
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ShowNewWaveInfo(WaveSpawnData waveData)
    {
        if (!waveImageGroup)
        {
            return;
        }

        headerPanel.TryGetComponent(out CanvasGroup headerCanvasGroup);
        if (headerCanvasGroup)
            headerCanvasGroup.DOFade(0, 1);

        ((RectTransform)wavePanelGroup.transform).DOAnchorPosX(-2000, 0).OnComplete(() =>
        {
            waveImageGroup.alpha = 0;
            wavePanelGroup.alpha = 0;
            wavePanelGroup.gameObject.SetActive(true);
        });

        wavePanelGroup.DOFade(1, 0.75f);
        var mySequence01 = DOTween.Sequence().SetRecyclable(true);
        mySequence01.Append(((RectTransform)wavePanelGroup.transform).DOAnchorPosX(0, 0.5f));
        mySequence01.OnComplete(() =>
        {
            foreach (var enemyData in waveData.enemySpawnData)
            {
                var m = Instantiate(enemySpawnMarker, wavePanelGroup.transform);
                m.ShowMarker(LevelManager.Instance.GetSpawnTransform(enemyData.spawnLocation), 5);
            }

            var mySequence02 = DOTween.Sequence().SetRecyclable(true);
            mySequence02.Append(waveImageGroup.DOFade(1, 0.5f));
            mySequence02.Append(waveImageGroup.DOFade(0, 0.5f));
            mySequence02.SetLoops(3);

            mySequence02.OnComplete(() =>
            {
                wavePanelGroup.DOFade(0, 0.25f);
                ((RectTransform)wavePanelGroup.transform).DOAnchorPosX(2000, 0.5f).OnComplete(() =>
                {
                    waveImageGroup.alpha = 0;
                    wavePanelGroup.gameObject.SetActive(false);

                    if (headerCanvasGroup)
                        headerCanvasGroup.DOFade(1, 0.5f);
                });
            });
        });
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