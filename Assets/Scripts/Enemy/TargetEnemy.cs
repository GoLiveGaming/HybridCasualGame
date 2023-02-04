using UnityEngine;
public class TargetEnemy : MonoBehaviour
{
    public GameObject gameObjectSelf { get { return gameObject; } }
    public Stats stats;
    public Canvas canvas;

    private void Awake()
    {
        if (!stats) GetComponent<Stats>();
    }

    private void FixedUpdate()
    {
        canvas.transform.LookAt(Camera.main.transform);
    }
}
