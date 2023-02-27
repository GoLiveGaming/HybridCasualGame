using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]private RectTransform levelPanel;
    [SerializeField]private RectTransform quitPanel;
    [SerializeField]private TMP_Text levelText;
    [SerializeField] private LevelLoader levelLoader;


    private void Awake()
    {
        Time.timeScale = 1;
#if UNITY_STANDALONE
        Screen.SetResolution(450, 750, true);

#endif
    }

    private void Start()
    {

    }

    public void OnStartVoid(Button btn)
    {
        btn.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.3f).OnComplete(() =>
        {
            levelPanel.gameObject.SetActive(true);
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
            {
                levelText.text = "Level 1";
            }
            else if (PlayerPrefs.GetInt("CurrentLevel") == 1)
            {
                levelText.text = "Level 2";
            }
            else if (PlayerPrefs.GetInt("CurrentLevel") == 2)
            {
                levelText.text = "Level 3";
            }
            else if (PlayerPrefs.GetInt("CurrentLevel") == 3)
            {
                levelText.text = "Level 4";
            }

            btn.transform.localScale = Vector3.one;
        }); 
    }

    public void OnQuitPanelVoid()
    {
        quitPanel.gameObject.SetActive(true);
        quitPanel.transform.GetChild(1).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        quitPanel.transform.GetChild(1).DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).OnComplete(() =>
        {
            quitPanel.transform.GetChild(1).DOScale(new Vector3(1f, 1f, 1f), 0.15f);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ClearAllPlayerPrefs()
    {
        PlayerDataManager.Instance.ClearAllPlayerPrefs();
        QuitGame();
        //quitPanel.gameObject.SetActive(false);

    }

    public void OnEnterVoid(Button btn)
    {
        btn.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.3f).OnComplete(() =>
        {
            int tempInt = PlayerPrefs.GetInt("CurrentLevel");
            //if (tempInt < 4)
                tempInt++;
            SceneManager.LoadSceneAsync(tempInt);
            Debug.Log(tempInt);
            //levelLoader.LoadNextLevel(tempInt);
            btn.transform.localScale = Vector3.one;
        });
        
    }


}
