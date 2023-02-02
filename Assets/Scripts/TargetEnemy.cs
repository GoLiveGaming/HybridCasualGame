
using UnityEngine;

public class TargetEnemy : MonoBehaviour
{
    public GameObject gameObjectSelf { get { return this.gameObject; } }

    public Stats stats;

    private void Awake()
    {
        if (!stats) GetComponent<Stats>();
    }

}
