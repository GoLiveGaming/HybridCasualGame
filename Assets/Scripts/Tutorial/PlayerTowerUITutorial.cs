

public class PlayerTowerUITutorial : PlayerTowerUI
{
    public override void ToggleUpgradesPanel()
    {
        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.currentlyOnStep == TutorialManager.TutorialStep.STEP_TWO)
            {
                StartCoroutine(tutorialManager.TutorialThirdStep());

                base.ToggleUpgradesPanel();
            }
        }
    }
}
