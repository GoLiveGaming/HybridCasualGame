using UnityEngine;
using UnityEngine.AI;
public abstract class NPCBaseState
{
    public abstract void EnterState(NPCStateManager npcSM, Animator animator, GameObject player);
    public abstract void UpdateState(NPCStateManager npcSM, Animator animator, GameObject player);
    public abstract void ExitState(NPCStateManager npcSM, Animator animator, GameObject player);

}
