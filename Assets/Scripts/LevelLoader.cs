using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    [SerializeField] private string[] allPlayableLevelsName;
    [SerializeField] private Animator Anim;
    [SerializeField] private float TransitionTime = 1;

    public string[] AllPlayableLevelsName
    {
        get
        {
            if (allPlayableLevelsName == null)
                Debug.LogError("Improper scene assignment on: " + this);
            return allPlayableLevelsName;
        }
    }

    public void LoadScene(string LevelName)
    {
        StartCoroutine(LoadSceneByName(LevelName));
    }
    public void LoadScene(int LevelIndex)
    {
        StartCoroutine(LoadSceneByIndex(LevelIndex));
    }
    public void LoadGameLevel(int LevelIndex)
    {
        StartCoroutine(LoadLevel(LevelIndex));
    }

    IEnumerator LoadSceneByName(string LevelName)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");

        Debug.Log("Scene To Load: " + LevelName);
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(LevelName);
        while (!Async.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadSceneByIndex(int LevelIndex)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");

        Debug.Log("Scene To Load Index: " + LevelIndex);
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(LevelIndex);
        while (!Async.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");

        Debug.Log("Scene To Load: " + AllPlayableLevelsName[LevelIndex]);
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(AllPlayableLevelsName[LevelIndex]);
        while (!Async.isDone)
        {
            yield return null;
        }
    }
}
