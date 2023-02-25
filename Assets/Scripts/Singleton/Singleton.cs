using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Debug.LogWarning(this + " is a Duplicate Instance of the class.");
        }
    }
    private void OnDestroy()
    {

        if (_instance == this)
        {
            _instance = null;
        }
    }
}
