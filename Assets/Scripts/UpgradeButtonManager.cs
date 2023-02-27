using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonManager : MonoBehaviour
{
    public UpgradeButton[] buttons;
    public StartSceneManager startSceneManager;

    private void OnEnable()
    {
        foreach (var child in buttons)
        {
            child.initialize();
        }

    }
}
