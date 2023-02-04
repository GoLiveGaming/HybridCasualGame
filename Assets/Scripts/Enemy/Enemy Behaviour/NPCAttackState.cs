using UnityEngine;

public class NPCAttackState : NPCBaseState
{
    public override void EnterState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        npcSM.activeState = NPCStateManager.NPCStates.Attack;

        npcSM.ResetAnimatorBools();
        npcSM.agent.isStopped = true;
        npcSM.agent.ResetPath();

        animator.SetTrigger("Attacking");
    }
    public override void UpdateState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        Debug.Log("Attacking");

    }

    public override void ExitState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        animator.ResetTrigger("Attacking");

    }
}
