using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCentralControl : MonoBehaviour
{
    NPCStateManager[] npc;
    private void Start()
    {
        npc = GetComponentsInChildren<NPCStateManager>();
    }
    void Update()
    {
        foreach (var npc in npc)
        {
            if (npc != null)
            {
                npc.updateNPC();
            }
        }
    }
}
