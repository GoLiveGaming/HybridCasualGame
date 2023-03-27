public class PlayerUnitDeployementAreaTutorial : PlayerUnitDeploymentArea
{
    #region Tutorial Stuff
    public override void UpgradeExistingAttackUnit(AttackType unitType)
    {
        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.completedTutorialSteps == TutorialManager.TutorialStep.STEP_THREE)
            {
                StartCoroutine(tutorialManager.TutorialFourthStep());

                base.UpgradeExistingAttackUnit(unitType);
            }
        }
    }

    #endregion
}
