
public class MainPlayerControlTutorial : MainPlayerControl
{
    #region Tutorial Stuff
    protected override void StartUpgradesProcess(PlayerUnitDeploymentArea area)
    {
        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.completedTutorialSteps == TutorialManager.TutorialStep.STEP_TWO)
            {
                StartCoroutine(tutorialManager.TutorialThirdStep());

                base.StartUpgradesProcess(area);
            }
        }
    }
    #endregion
}