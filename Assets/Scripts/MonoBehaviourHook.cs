/*
 * Class to hook Actions into MonoBehaviour
 * */
using UnityEngine;

public class MonoBehaviourHook : MonoBehaviour
{

    public System.Action OnUpdate;

    private void FixedUpdate()
    {
        if (OnUpdate != null) OnUpdate();
    }
}