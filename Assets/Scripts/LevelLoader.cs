using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    [SerializeField] private Object[] allPlayableLevels;
    [SerializeField] private Animator Anim;
    [SerializeField] private float TransitionTime = 1;

    public Object[] AllPlayableLevels
    {
        get
        {
            if (allPlayableLevels == null)
                Debug.LogError("Improper scene assignment on: " + this);
            return allPlayableLevels;
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

        Debug.Log("Scene To Load: " + AllPlayableLevels[LevelIndex].name);
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(AllPlayableLevels[LevelIndex].name);
        while (!Async.isDone)
        {
            yield return null;
        }
    }
}
