using DG.Tweening;
using TMPro;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("PANELS REFRENCES"), Space(2)]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject unlocksPanel;
    [SerializeField] private GameObject quitGamePanel;

    [Header("TEXT FIELDS REFRENCES"), Space(2)]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text coinsAmountText;

    [Header("CLASS DEPENDENCIES REFRENCES"), Space(2)]
    [SerializeField] private LevelLoader levelLoader;

    [Header("READONLY"), Space(2)]
    [SerializeField, ReadOnly] private RectTransform activePanel;


    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        if (!levelLoader) levelLoader = FindObjectOfType<LevelLoader>();
    }
    public void UpdateCoinsAmountText()
    {
        if (!coinsAmountText) return;
        coinsAmountText.text = PlayerDataManager.Instance.CoinsAmount.ToString();
    }




    #region BUTTON_EVENTS


    public void ToggleLevelSelectPanel()
    {
        if (!levelSelectPanel) { Debug.LogWarning("LevelSelectPanel is not assigned at: " + this); return; }


        if (levelSelectPanel.TryGetComponent(out EnhancedPanels panel))
        {
            panel.TogglePanel();
        }
        else
        {
            levelSelectPanel.SetActive(!levelSelectPanel.activeSelf);
        }

        int tempInt = PlayerPrefs.GetInt("CurrentLevel");
        levelText.text = "Level " + (tempInt + 1);
    }

    public void ToggleUnlocksPanel()
    {
        if (!unlocksPanel) { Debug.LogWarning("UnlocksPanel is not assigned at: " + this); return; }


        if (unlocksPanel.TryGetComponent(out EnhancedPanels panel))
        {
            panel.TogglePanel();
        }
        else
        {
            unlocksPanel.SetActive(!levelSelectPanel.activeSelf);
        }
    }
    public void ToggleQuitPanel()
    {
        if (!quitGamePanel) { Debug.LogWarning("QuitGamePanel Panel is not assigned at: " + this); return; }

        if (quitGamePanel.TryGetComponent(out EnhancedPanels panel))
        {
            panel.TogglePanel();
        }
        else
        {
            quitGamePanel.SetActive(!levelSelectPanel.activeSelf);
        }
    }



    public void LoadNextLevel()
    {
        int tempInt = PlayerPrefs.GetInt("CurrentLevel");
        //if (tempInt < 4)
        tempInt++;
        Debug.Log("Scene To Load Index: " + tempInt);
        levelLoader.LoadNextLevel(tempInt);
    }


    public void ClearAllPlayerPrefs()
    {
        PlayerDataManager.Instance.ClearAllPlayerPrefs();
        QuitGame();
        //quitPanel.gameObject.SetActive(false);

    }


    public void QuitGame()
    {
        Application.Quit();
    }


    #endregion
}


