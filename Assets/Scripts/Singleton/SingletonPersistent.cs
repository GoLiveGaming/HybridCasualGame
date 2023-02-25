using UnityEngine;
public class SingletonPersistent<T> : MonoBehaviour where T : Component
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
            Debug.LogWarning(this + " is a Duplicate Intance of the class. Destroying duplicate Instance");
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {

        if (_instance == this)
        {
            _instance = null;
        }
    }
}
