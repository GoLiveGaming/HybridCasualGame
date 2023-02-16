using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Rect Componenets")]
    public GameObject unitSelectionCanvas;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Button Componenets")]
    public Button pauseBtn;
    public Button resumeBtn;
    public Button restartBtn;
    public Button quitBtn;

    [Header("Image Componenets")]
    public Image unitSelectionCooldownTimerImage;

    [Header("Text Componenets")]
    public TMP_Text waveTxt;
    public TMP_Text overTxt;
    public TMP_Text enemiesCountTxt;

    [SerializeField] private TextMeshProUGUI m_warningText;

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
    private void Awake()
    {
        Instance = this;
        pauseBtn.onClick.AddListener(PauseButton);
        resumeBtn.onClick.AddListener(ResumeButton);
        restartBtn.onClick.AddListener(RestartButton);
        quitBtn.onClick.AddListener(QuitButton);
    }
    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(PauseButton);
        resumeBtn.onClick.RemoveListener(ResumeButton);
        restartBtn.onClick.RemoveListener(RestartButton);
        quitBtn.onClick.RemoveListener(QuitButton);
    }
    internal void ShowText(string tempTxt)
    {
     //   ShowResponseMessage(tempTxt, waveTxt);
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
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeButton()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }
    public void RestartButton()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitButton()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void GameOverVoid(string textTemp, bool isWon)
    {
        gameOverPanel.SetActive(true);
        overTxt.text = textTemp;

        if (isWon && (PlayerPrefs.GetInt("CurrentLevel") < 2))
        {
            int tempInt = PlayerPrefs.GetInt("CurrentLevel");
            tempInt++;
            PlayerPrefs.SetInt("CurrentLevel", tempInt);
        }
        Time.timeScale = 0;
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

}
