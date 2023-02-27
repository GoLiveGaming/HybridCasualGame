using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator Anim;
    public float TransitionTime = 1;

    // Update is called once per frame
    public void LoadNextLevel(string LevelName)
    {
        StartCoroutine(LoadLevel(LevelName));
    }
    public void LoadNextLevel(int Level)
    {
        StartCoroutine(LoadLevel(Level));
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
    
    IEnumerator LoadLevel(int Level)
    {
        Time.timeScale = 1.0f;
        Anim.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        AsyncOperation Async = SceneManager.LoadSceneAsync(Level);
        while(!Async.isDone)
        {
            yield return null;
        }
        //SceneManager.LoadScene(LevelName);
    }
}
