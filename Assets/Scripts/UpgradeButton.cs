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
    /*private void Start()
    {
        button = GetComponent<Button>();
        playerDataManager = PlayerDataManager.Instance;
        initialize();
    }*/


    public void initialize()
    {
        button = GetComponent<Button>();
        playerDataManager = PlayerDataManager.Instance;

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
            return;
        }

        ShakeButton();
    }

    private void ShakeButton()
    {
        (transform as RectTransform).DOShakeAnchorPos(0.5f, 15f);
    }

}

