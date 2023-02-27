using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    [ReadOnly] public bool isUnlocked = false;
    [ReadOnly] public Button button;

    public string attackName;

    PlayerDataManager playerDataManager;
    StartSceneManager startSceneManager;
    private void Start()
    {
        initialize();
    }


    public void initialize()
    {
        if (!button) button = GetComponent<Button>();
        if (!playerDataManager) playerDataManager = PlayerDataManager.Instance;
        if (!startSceneManager) startSceneManager = FindObjectOfType<StartSceneManager>();

        if (startSceneManager) startSceneManager.UpdateCoinsAmountText();
        isUnlocked = playerDataManager.IsAttackTypeUnlocked(playerDataManager.GetAttackTypeFromString(attackName));
        if (isUnlocked)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void TriggerFunctionality()
    {
        isUnlocked = playerDataManager.UnlockAttackType(attackName);

        if (isUnlocked)
        {
            button.interactable = false;
            if (startSceneManager) startSceneManager.UpdateCoinsAmountText();
            return;
        }

        ShakeButton();
    }

    private void ShakeButton()
    {
        (transform as RectTransform).DOShakeAnchorPos(0.5f, 15f);
    }

}

