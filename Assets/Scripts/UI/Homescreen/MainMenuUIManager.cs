using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using NaughtyAttributes;
public class MainMenuUIManager : MonoBehaviour
{
    [Header("PANELS REFRENCES"), Space(2)]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject unlocksPanel;
    [SerializeField] private GameObject quitGamePanel;

    [Header("TEXT FIELDS REFRENCES"), Space(2)]
    [SerializeField] private TMP_Text selectedLevelText;
    [SerializeField] private TMP_Text coinsAmountText;

    [Header("AUDIO MIXERS REFRENCES"), Space(2)]
    [SerializeField] private AudioMixer fxMixer;

    [Header("CLASS DEPENDENCIES REFRENCES"), Space(2)]
    [SerializeField, ReadOnly] private LevelLoader levelLoader;
    [SerializeField, ReadOnly] private PlayerDataManager playerDataManager;


    public ScrollRect levelSelectScrollRect;
    public LevelSelectItemView[] levelSelectionItems;
    private LevelSelectItemView activeLevelSelectItem;
    private float scrollPosition;
    private void Awake()
    {
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        levelLoader = LevelLoader.Instance;
        playerDataManager = PlayerDataManager.Instance;

        levelSelectScrollRect.onValueChanged.AddListener(SetActiveLevel);
        InitializeLevelSelection();
        SetActiveLevel(Vector2.zero);

        UpdateAllTextFields();
    }
    private void OnDestroy()
    {
        levelSelectScrollRect.onValueChanged.RemoveListener(SetActiveLevel);
    }

    #region  Text Fields Update
    public void UpdateAllTextFields()
    {
        if (coinsAmountText)
        {
            coinsAmountText.text = playerDataManager.CoinsAmount.ToString();
        }
        if (selectedLevelText) selectedLevelText.text = playerDataManager.SelectedLevelIndex.ToString();
    }
    public void UpdateCoinsAmountText()
    {
        if (coinsAmountText)
        {
            coinsAmountText.text = playerDataManager.CoinsAmount.ToString();
        }
    }
    public void UpdateSelectedLevelText()
    {
        if (selectedLevelText) selectedLevelText.text = playerDataManager.SelectedLevelIndex.ToString();
    }

    private void InitializeLevelSelection()
    {
        for (int i = 0; i < levelSelectionItems.Length; i++)
        {
            if (i <= playerDataManager.UnlockedLevelsCount)
                levelSelectionItems[i].Initialize(true);
            else
                levelSelectionItems[i].Initialize(false);
        }
    }
    private void SetActiveLevel(Vector2 value)
    {
        // Calculate the current scroll position based on the horizontal scrollbar's value
        scrollPosition = Mathf.Clamp01(value.x);

        // Calculate the index of the nearest item based on the current scroll position
        int nearestIndex = Mathf.RoundToInt(scrollPosition * (levelSelectionItems.Length - 1));

        // Select the nearest item
        SelectLevelSelectItem(nearestIndex);
    }



    private void SelectLevelSelectItem(int index)
    {
        if (activeLevelSelectItem != null && activeLevelSelectItem != levelSelectionItems[index])
        {
            activeLevelSelectItem.Deselect();
        }

        activeLevelSelectItem = levelSelectionItems[index];
        activeLevelSelectItem.Select();
        UpdateSelectedLevelText();
        if (levelSelectionItems[index].isUnlocked)
            playerDataManager.SelectedLevelIndex = index;
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
        InitializeLevelSelection();
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
    }
    public void SelectPreviousLevel()
    {
        int lvl = playerDataManager.SelectedLevelIndex - 1;
        if (lvl < 0)
        {
            lvl = playerDataManager.UnlockedLevelsCount;
        }
        playerDataManager.SelectedLevelIndex = lvl;
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


