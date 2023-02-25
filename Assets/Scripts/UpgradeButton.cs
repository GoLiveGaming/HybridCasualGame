using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    [ReadOnly] public bool isInteractable = false;
    [ReadOnly] public Button button;

    public string attackName;

    PlayerDataManager playerDataManager;
    private void Start()
    {
        button = GetComponent<Button>();
        playerDataManager = PlayerDataManager.Instance;
        initialize();
    }

    void initialize()
    {
        isInteractable = playerDataManager.IsAttackTypeUnlocked(playerDataManager.GetAttackTypeFromString(attackName));
        if (isInteractable)
        {
            button.interactable = false;
        }
    }
    public void TriggerFunctionality()
    {
        isInteractable = playerDataManager.UnlockAttackType(attackName);

        if (isInteractable)
        {
            button.interactable = false;
            return;
        }

        ShakeButton();
    }

    private void ShakeButton()
    {
        (transform as RectTransform).DOShakeAnchorPos(0.5f, 15f);
    }

}

