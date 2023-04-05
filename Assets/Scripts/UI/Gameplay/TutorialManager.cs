using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : UIManager
{


    [Space(10), Header("TUTORIAL PARAMETERS"), Space(5)]
    public GameObject blackPanel;
    public Button pauseBtn;

    public TextMeshProUGUI tutorialTextSmall;
    public TextMeshProUGUI tutorialTextBig;

    public PlayerUnitDeploymentArea[] deployAreas;

    public TowerDeployeButtonTutorial[] deployButtons;

    public GameObject[] manaObjects;

    public Renderer[] mainBarracksMaterials;

    public Animator cameraAnimator;
    public Animator mainBarracks;

    [ReadOnly] public TutorialStep currentlyOnStep;

    public enum TutorialStep { NONE, STEP_ONE, STEP_TWO, STEP_THREE, STEP_FOUR };
    public override void Start()
    {
        base.Start();
        blackPanel.SetActive(false);
        StartCoroutine(TutorialFirstStep());
    }

    private IEnumerator TutorialFirstStep()
    {

        ToggleDeployButtons(false);
        ChangeDeployedAreaState(0);

        pauseBtn.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
        ShowTextSmall("Skeletons are coming for your Kingdom!");
        cameraAnimator.gameObject.SetActive(true);
        cameraAnimator.SetBool("PlayAnim1", true);

        yield return new WaitForSeconds(5f);
        cameraAnimator.gameObject.SetActive(false);
        cameraAnimator.SetBool("PlayAnim1", false);
        tutorialTextSmall.text = "";

        yield return new WaitForSeconds(1f);
        tutorialTextBig.gameObject.SetActive(true);
        blackPanel.SetActive(true);

        tutorialTextBig.text = "Let's bring in a Wizard to help defend your Castle!";
        ShowTextSmall("Place your Wizard Tower where it can protect your Castle");

        LevelManager.Instance.PauseEnemies = true;

        ChangeDeployedAreaState(2);
        EndFirstStep();

        currentlyOnStep = TutorialStep.STEP_ONE;
    }
    public IEnumerator TutorialSecondStep()
    {

        ToggleDeployButtons(true);

        tutorialTextBig.gameObject.SetActive(false);

        LevelManager.Instance.PauseEnemies = false;

        Utils.isGamePaused = false;
        for (int i = 0; i < deployAreas.Length; i++)
        {
            deployAreas[i].gameObject.SetActive(true);
        }


        ShowTextSmall("Survive the skeleton armies! If your castle falls - you lose!", 1);
        cameraAnimator.gameObject.SetActive(true);
        cameraAnimator.SetBool("PlayAnim2", true);
        mainBarracks.enabled = true;
        foreach (var t in mainBarracksMaterials)
        {
            t.material.EnableKeyword("_EMISSION");
        }

        yield return new WaitForSeconds(5f);

        cameraAnimator.gameObject.SetActive(false);
        tutorialTextSmall.text = "";
        foreach (var t in mainBarracksMaterials)
        {
            t.material.DisableKeyword("_EMISSION");
        }
        mainBarracks.enabled = false;

        yield return new WaitForSeconds(5f);

        tutorialTextBig.gameObject.SetActive(true);
        tutorialTextBig.text = "Combine Spells to make stronger Towers!";

        foreach (var area in deployAreas)
        {
            if (!area.HasDeployedUnit)
            {
                continue;
            }

            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var renderers = area.GetComponentsInChildren<Renderer>();
            foreach (var rnd in renderers)
            {
                if (rnd) rnd.material.EnableKeyword("_EMISSION");
            }
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        ChangeDeployedAreaState(2);

        LevelManager.Instance.PauseEnemies = true;

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        EndSecondStep();
        currentlyOnStep = TutorialStep.STEP_TWO;
    }

    public IEnumerator TutorialThirdStep()
    {
        LevelManager.Instance.PauseEnemies = true;
        ToggleDeployButtons(false);


        tutorialTextBig.gameObject.SetActive(true);
        tutorialTextBig.text = "Mix the tower with another element to make new Magic!";

        yield return new WaitForSeconds(4f);
        tutorialTextBig.gameObject.SetActive(false);

        currentlyOnStep = TutorialStep.STEP_THREE;
    }
    public IEnumerator TutorialFourthStep()
    {

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        ToggleDeployButtons(true);

        LevelManager.Instance.PauseEnemies = false;
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        ChangeDeployedAreaState(2);

        pauseBtn.gameObject.SetActive(true);
        yield return null;
        LevelManager.Instance.PauseEnemies = false;
        currentlyOnStep = TutorialStep.STEP_FOUR;
    }

    public void ToggleDeployButtons(bool value)
    {
        for (int i = 0; i < deployButtons.Length; i++)
        {
            deployButtons[i].ableToDrag = value;
            deployButtons[i].transform.GetChild(0).gameObject.SetActive(value);
            deployButtons[i].GetComponent<Button>().interactable = value;
        }
    }

    public void EndFirstStep()
    {
        foreach (TowerDeployeButtonTutorial button in deployButtons)
        {
            if (button.attackType == AttackType.FireAttack)
            {
                button.ableToDrag = true;
                button.GetComponent<Button>().interactable = true;
                button.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                button.ableToDrag = false;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void EndSecondStep()
    {
        foreach (TowerDeployeButtonTutorial button in deployButtons)
        {
            button.ableToDrag = false;
            button.GetComponent<Button>().interactable = false;

        }
    }

    public void ChangeDeployedAreaState(int changeState)
    {
        for (int i = 0; i < deployAreas.Length; i++)
        {
            if (changeState == 0 && deployAreas[i].GetComponent<PlayerUnitDeploymentArea>().deployedTower)
            {
                deployAreas[i].transform.GetComponent<BoxCollider>().enabled = false;
            }
            else if (changeState == 1)
            {
                if (deployAreas[i].transform.GetComponent<BoxCollider>().enabled == true)
                    deployAreas[i].transform.GetComponent<BoxCollider>().enabled = false;
                else
                    deployAreas[i].transform.GetComponent<BoxCollider>().enabled = true;
            }
            else if (changeState == 2)
            {
                deployAreas[i].transform.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    private void ShowTextSmall(string tempTxt, float duration = 0.25f)
    {
        if (!tutorialTextSmall)
            return;
        tutorialTextSmall.fontSize = 70f;
        tutorialTextSmall.gameObject.SetActive(true);
        tutorialTextSmall.text = tempTxt;
        tutorialTextSmall.transform.localScale = Vector3.zero;
        tutorialTextSmall.transform.DOScale(Vector3.one, duration);
    }

}
