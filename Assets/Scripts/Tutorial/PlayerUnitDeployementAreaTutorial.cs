public class PlayerUnitDeployementAreaTutorial : PlayerUnitDeploymentArea
{
    #region Tutorial Stuff
    public override void UpgradeExistingAttackUnit(AttackType unitToDeployType)
    {
        if (UIManager.Instance.TryGetComponent(out TutorialManager tutorialManager))
        {
            if (tutorialManager.currentlyOnStep == TutorialManager.TutorialStep.STEP_THREE)
            {
                StartCoroutine(tutorialManager.TutorialFourthStep());

                base.UpgradeExistingAttackUnit(unitToDeployType);
            }
        }
    }

    #endregion
}
