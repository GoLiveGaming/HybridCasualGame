using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Instance;
    public static bool isGamePaused = false;

    private void Awake()
    {
        Instance = this;
    }
}
