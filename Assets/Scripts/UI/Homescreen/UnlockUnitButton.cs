using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Button))]
public class UnlockUnitButton : MonoBehaviour
{
    [ReadOnly] public bool isUnlocked = false;
    [ReadOnly] public Button button;
    public AttackType attackType;
    [SerializeField] private GameObject confirmationPanel;
    MainPlayerControl _mainPlayerControls;
    MainMenuUIManager mainMenuUIManager;
    private void Start()
    {
        initialize();
    }


    public void initialize()
    {
        if (!button) button = GetComponent<Button>();
        if (!_mainPlayerControls) _mainPlayerControls = MainPlayerControl.Instance;
        if (!mainMenuUIManager) mainMenuUIManager = FindObjectOfType<MainMenuUIManager>();

        if (mainMenuUIManager) mainMenuUIManager.UpdateCoinsAmountText();
        isUnlocked = _mainPlayerControls.IsAttackTypeUnlocked(attackType);
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
        isUnlocked = _mainPlayerControls.UnlockAttack(attackType);

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

