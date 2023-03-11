using TMPro;
using UnityEngine;
using UnityEngine.Audio;
public class MainMenuUIManager : MonoBehaviour
{
    [Header("PANELS REFRENCES"), Space(2)]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject unlocksPanel;
    [SerializeField] private GameObject quitGamePanel;

    [Header("TEXT FIELDS REFRENCES"), Space(2)]
    [SerializeField] private TMP_Text selectedLevelText;
    [SerializeField] private TMP_Text coinsAmountText;
    [SerializeField] private TMP_Text coinsAmountTextLevelsPanel;
    [SerializeField] private TMP_Text coinsAmountTextStartPanel;

    [Header("AUDIO MIXERS REFRENCES"), Space(2)]
    [SerializeField] private AudioMixer fxMixer;

    [Header("CLASS DEPENDENCIES REFRENCES"), Space(2)]
    [SerializeField, ReadOnly] private LevelLoader levelLoader;
    [SerializeField, ReadOnly] private PlayerDataManager playerDataManager;

    private void Awake()
    {
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        levelLoader = LevelLoader.Instance;
        playerDataManager = PlayerDataManager.Instance;

        UpdateAllTextFields();
    }

    #region  Text Fields Update
    public void UpdateAllTextFields()
    {
        if (coinsAmountText)
        {
            coinsAmountText.text = playerDataManager.CoinsAmount.ToString();
            coinsAmountTextLevelsPanel.text = playerDataManager.CoinsAmount.ToString();
            coinsAmountTextStartPanel.text = playerDataManager.CoinsAmount.ToString();
        }
        if (selectedLevelText) selectedLevelText.text = playerDataManager.SelectedLevelIndex.ToString();
    }
    public void UpdateCoinsAmountText()
    {
        if (coinsAmountText)
        {
            coinsAmountText.text = playerDataManager.CoinsAmount.ToString();
            coinsAmountTextLevelsPanel.text = playerDataManager.CoinsAmount.ToString();
            coinsAmountTextStartPanel.text = playerDataManager.CoinsAmount.ToString();
        }
    }
    public void UpdateSelectedLevelText()
    {
        if (selectedLevelText) selectedLevelText.text = playerDataManager.SelectedLevelIndex.ToString();
    }


    #endregion

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

        UpdateSelectedLevelText();
        UpdateCoinsAmountText();
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
            unlocksPanel.SetActive(!unlocksPanel.activeSelf);
        }
        
        UpdateCoinsAmountText();
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
            quitGamePanel.SetActive(!quitGamePanel.activeSelf);
        }
    }
    public void LoadPlayableLevel()
    {
       // playerDataManager.SelectedLevelIndex = 1;
        levelLoader.LoadGameLevel(playerDataManager.SelectedLevelIndex);
    }

    public void SelectNextLevel()
    {
        int lvl = playerDataManager.SelectedLevelIndex + 1;
        if (lvl > playerDataManager.UnlockedLevelsCount)
        {
            lvl = 0;
        }
        playerDataManager.SelectedLevelIndex = lvl;

        UpdateSelectedLevelText();
    }
    public void SelectPreviousLevel()
    {
        int lvl = playerDataManager.SelectedLevelIndex - 1;
        if (lvl < 0)
        {
            lvl = playerDataManager.UnlockedLevelsCount;
        }
        playerDataManager.SelectedLevelIndex = lvl;

        UpdateSelectedLevelText();
    }
    public void MuteFX()
    {
        fxMixer.GetFloat("FXVolume", out float CurrentLevel);

        if (CurrentLevel >= 0)
            fxMixer.SetFloat("FXVolume", -80f);
        else
            fxMixer.SetFloat("FXVolume", 0f);
    }

    public void ClearAllPlayerPrefs()
    {
        PlayerDataManager.Instance.ClearAllPlayerPrefs();
        QuitGame();
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    #endregion
}


