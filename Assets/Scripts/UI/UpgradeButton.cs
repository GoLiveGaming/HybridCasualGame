using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    [ReadOnly] public bool isUnlocked = false;
    [ReadOnly] public Button button;
    public AttackType attackType;
    [SerializeField] private GameObject confirmationPanel;
    PlayerDataManager playerDataManager;
    MainMenuUIManager mainMenuUIManager;
    private void Start()
    {
        initialize();
    }


    public void initialize()
    {
        if (!button) button = GetComponent<Button>();
        if (!playerDataManager) playerDataManager = PlayerDataManager.Instance;
        if (!mainMenuUIManager) mainMenuUIManager = FindObjectOfType<MainMenuUIManager>();

        if (mainMenuUIManager) mainMenuUIManager.UpdateCoinsAmountText();
        isUnlocked = playerDataManager.IsAttackTypeUnlocked(attackType);
        if (isUnlocked)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void ToggleConfirmationPanel(bool toggle)
    {
        if (confirmationPanel) confirmationPanel.SetActive(toggle);
    }
    public void TriggerFunctionality()
    {
        isUnlocked = playerDataManager.UnlockAttackType(attackType);

        if (isUnlocked)
        {
            button.interactable = false;
            if (mainMenuUIManager) mainMenuUIManager.UpdateCoinsAmountText();
            ShakeButton(5);
            return;
        }

        ShakeButton(15);
    }

    private void ShakeButton(float intensity)
    {
        (transform as RectTransform).DOShakeAnchorPos(0.5f, intensity);
    }

}

