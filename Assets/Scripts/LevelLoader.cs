using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator Anim;
    public float TransitionTime = 1;

    public void LoadNextLevel(string LevelName)
    {
        StartCoroutine(LoadLevel(LevelName));
    }
    public void LoadNextLevel(int LevelIndex)
    {
        StartCoroutine(LoadLevel(LevelIndex));
    }

    IEnumerator LoadLevel(string LevelName)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(LevelName);
        while(!Async.isDone)
        {
            yield return null;
        }
        //SceneManager.LoadScene(LevelName);
    }
    
    IEnumerator LoadLevel(int LevelIndex)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(LevelIndex);
        while(!Async.isDone)
        {
            yield return null;
        }
        //SceneManager.LoadScene(LevelName);
    }
}
