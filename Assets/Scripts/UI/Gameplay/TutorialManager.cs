using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : UIManager
{
    internal bool firstStep, secondStep, thirdStep;

    public RectTransform TutorialPanelOne;
    public Button pauseBtn;

    public TMP_Text tutorialBouncyTxt;
    public TMP_Text tutorialBouncyTxtBig;

    public GameObject[] deployAreas;

    public TowerDeployeButtonTutorial[] deployButtons;

    public GameObject[] manaObjects;

    public Renderer[] mainBarracksMaterials;

    public Animator cameraAnimator;
    public Animator mainBarracks;

    public override void Start()
    {
        base.Start();
        TutorialPanelOne.gameObject.SetActive(false);
        StartCoroutine(StartTutorial());
    }
    IEnumerator StartTutorial()
    {
        pauseBtn.gameObject.SetActive(false);
        ShowDeployButtonsCost(false);
        ToggleDeployButtons(false);
        yield return new WaitForSeconds(4f);
        TutorialBouncyText("Skeletons are coming for your Kingdom!", 1);
        cameraAnimator.gameObject.SetActive(true);
        cameraAnimator.SetBool("PlayAnim1", true);
        yield return new WaitForSeconds(5f);
        cameraAnimator.gameObject.SetActive(false);
        cameraAnimator.SetBool("PlayAnim1", false);
        tutorialBouncyTxt.text = "";
        yield return new WaitForSeconds(1f);
        tutorialBouncyTxtBig.gameObject.SetActive(true);
        TutorialPanelOne.gameObject.SetActive(true);
        tutorialBouncyTxtBig.text = "Let's bring in a Wizard to help defend your Castle!";
        for (int i = 0; i < LevelManager.Instance.enemiesParent.childCount; i++)
        {
            LevelManager.Instance.enemiesParent.GetChild(i).GetComponent<NPCManagerScript>().SetMoveSpeed(0);
        }
        Utils.isGamePaused = true;
        //GlowDeployButtons(true);
        //ToggleDeployButtons(true);
        EnableDeployButtonsFirstStep();
    }
    public IEnumerator TutorialSecondStep()
    {
        ShowDeployButtonsCost(true);
        ToggleDeployButtons(true);

        tutorialBouncyTxtBig.gameObject.SetActive(false);
        
        for (int i = 0; i < LevelManager.Instance.enemiesParent.childCount; i++)
        {
            LevelManager.Instance.enemiesParent.GetChild(i).GetComponent<NPCManagerScript>().ResetMoveSpeed();
        }
        Utils.isGamePaused = false;
        for (int i = 0; i < deployAreas.Length; i++)
        {
            deployAreas[i].SetActive(true);
        }
        TutorialBouncyText("Placing a Wizard Tower Costs Mana", 1);
        manaObjects[0].SetActive(true);
        manaObjects[1].SetActive(true);
        yield return new WaitForSeconds(10f);
        tutorialBouncyTxt.text = "";
        manaObjects[0].SetActive(false);
        manaObjects[1].SetActive(false);
        yield return new WaitForSeconds(3f);
        TutorialBouncyText("Your Wizard Tower will repel the skeleton horde!", 1);
        yield return new WaitForSeconds(6f);
        tutorialBouncyTxt.text = "";
        yield return new WaitForSeconds(3f);
        TutorialBouncyText("Survive the skeleton armies! If your castle falls - you lose!", 1);
        cameraAnimator.gameObject.SetActive(true);
        cameraAnimator.SetBool("PlayAnim2", true);
        mainBarracks.enabled = true;
        for (int i = 0; i < mainBarracksMaterials.Length; i++)
        {
            mainBarracksMaterials[0].material.EnableKeyword("_EMISSION");
        }
        yield return new WaitForSeconds(5f);
        cameraAnimator.gameObject.SetActive(false);
        tutorialBouncyTxt.text = "";
        for (int i = 0; i < mainBarracksMaterials.Length; i++)
        {
            mainBarracksMaterials[0].material.DisableKeyword("_EMISSION");
        }
        mainBarracks.enabled = false;
        yield return new WaitForSeconds(5f);
        tutorialBouncyTxtBig.gameObject.SetActive(true);
        tutorialBouncyTxtBig.text = "Combine Spells to make stronger Towers!";
        secondStep = true;
        ChangeDeployedAreas(1);
        for (int i = 0; i < LevelManager.Instance.enemiesParent.childCount; i++)
        {
            LevelManager.Instance.enemiesParent.GetChild(i).GetComponent<NPCManagerScript>().SetMoveSpeed(0);
        }
        Utils.isGamePaused = true;
        for (int k = 0; k < deployAreas.Length; k++)
        {
            if (deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower)
            {
                for (int i = 0; i < deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.childCount; i++)
                {
                    //  deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetComponent<Animator>().enabled = true;
                    deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).TryGetComponent(out Renderer renderer);
                    if (renderer) renderer.material.EnableKeyword("_EMISSION");

                    for (int j = 0; j < deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).childCount; j++)
                    {
                        deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).transform.GetChild(j).TryGetComponent(out Renderer rendererTemp);
                        if (rendererTemp) rendererTemp.material.EnableKeyword("_EMISSION");
                    }
                }
            }
        }
        TutorialPanelOne.gameObject.SetActive(true);
        //GlowDeployButtons(true);
        EnableDeployButtonsSecondStep();
    }

    public IEnumerator TutorialThirdStep()
    {
        ShowDeployButtonsCost(true);
        ToggleDeployButtons(true);

        tutorialBouncyTxtBig.gameObject.SetActive(false);

        for (int i = 0; i < LevelManager.Instance.enemiesParent.childCount; i++)
        {
            LevelManager.Instance.enemiesParent.GetChild(i).GetComponent<NPCManagerScript>().ResetMoveSpeed();
        }
        Utils.isGamePaused = false;
        ChangeDeployedAreas(2);
        for (int k = 0; k < deployAreas.Length; k++)
        {
            if (deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower)
            {
                deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.TryGetComponent(out Animator animator);
                if (animator)
                {
                    for (int i = 0; i < deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.childCount; i++)
                    {
                        //  animator.enabled = false;
                        deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).TryGetComponent(out Renderer renderer);
                        if (renderer) renderer.material.DisableKeyword("_EMISSION");

                        for (int j = 0; j < deployAreas[0].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).childCount; j++)
                        {
                            deployAreas[k].GetComponent<PlayerUnitDeploymentArea>().deployedTower.transform.GetChild(i).transform.GetChild(j).TryGetComponent(out Renderer rendererTemp);
                            if (rendererTemp) rendererTemp.material.DisableKeyword("_EMISSION");
                        }
                    }
                }
            }
        }
        ShowDeployButtonsCost(false);
        yield return new WaitForSeconds(5f);
        TutorialBouncyText("Different Elements can mix to create new effects!", 1);
        pauseBtn.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        tutorialBouncyTxt.text = "";
    }
    public void ShowDeployButtonsCost(bool value)
    {
        for (int i = 0; i < deployButtons.Length; i++)
        {
            deployButtons[i].transform.GetChild(0).gameObject.SetActive(value);
        }
    }
    public void ToggleDeployButtons(bool value)
    {
        for (int i = 0; i < deployButtons.Length; i++)
        {
            deployButtons[i].ableToDrag = value;
            deployButtons[i].GetComponent<Button>().interactable = value;
        }
    }

    public void EnableDeployButtonsFirstStep()
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

    public void EnableDeployButtonsSecondStep()
    {
        foreach (TowerDeployeButtonTutorial button in deployButtons)
        {
            if (button.attackType == AttackType.WindAttack)
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

    public void ChangeDeployedAreas(int changeState)
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
    public void TempThree()
    {
        Utils.isGamePaused = true;
    }
    internal void TutorialBouncyText(string tempTxt, int duration)
    {
        if (!tutorialBouncyTxt)
            return;
        tutorialBouncyTxt.fontSize = 70f;
        tutorialBouncyTxt.gameObject.SetActive(true);
        tutorialBouncyTxt.text = tempTxt;
        tutorialBouncyTxt.transform.localScale = Vector3.one;
        tutorialBouncyTxt.transform.DOScale(Vector3.one * 1.1f, duration).OnComplete(() =>
        {
            // tutorialBouncyTxt.text = "";
            // tutorialBouncyTxt.fontSize = 100f;
            // tutorialBouncyTxt.transform.localScale = Vector3.one;
        });
    }

}
