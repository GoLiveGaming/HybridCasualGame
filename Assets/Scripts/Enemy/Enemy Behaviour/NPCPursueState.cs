using UnityEngine;
public class NPCPursueState : NPCBaseState
{
    public override void EnterState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        npcSM.activeState = NPCStateManager.NPCStates.Pursue;
        if (npcSM.doAnims)
        {
            npcSM.ResetAnimatorBools();
        }
        npcSM.agent.isStopped = true;
        npcSM.agent.ResetPath();

        npcSM.agent.SetDestination(npcSM.GetTargetLocation());
        npcSM.animator.SetTrigger("Running");
    }
    public override void UpdateState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        if (npcSM.HasRechedDestination())
        {
            npcSM.agent.ResetPath();
            ExitState(npcSM, animator, player);
        }
    }
    public override void ExitState(NPCStateManager npcSM, Animator animator, GameObject player)
    {
        npcSM.animator.ResetTrigger("Running");
        npcSM.SwitchState(npcSM.attackState);
    }

}
