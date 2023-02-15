using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]private RectTransform levelPanel;
    [SerializeField]private TMP_Text levelText;


    private void Awake()
    {
        Time.timeScale = 1;
#if UNITY_STANDALONE
        Screen.SetResolution(450, 750, true);

#endif
    }

    public void OnStartVoid()
    {
        
        levelPanel.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            levelText.text = "Level 1";
        }
        else if(PlayerPrefs.GetInt("CurrentLevel") == 1)
        {
            levelText.text = "Level 2";
        }
        else if (PlayerPrefs.GetInt("CurrentLevel") == 2)
        {
            levelText.text = "Level 3";
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnEnterVoid()
    {
          SceneManager.LoadSceneAsync(1);
    }


}
