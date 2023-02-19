using UnityEngine;
public class NPCAttackState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Attack;
        npcManager._agent.isStopped = true;
        npcManager._agent.ResetPath();
        npcManager.m_Animator.SetBool("Attack", true);
    }
    public override void UpdateState(NPCManagerScript npcManager)
    {
        if (!npcManager.InTargetProximity())
        {
            npcManager.UpdateDestination();
            ExitState(npcManager);
        }
    }

    public override void ExitState(NPCManagerScript npcManager)
    {
        npcManager.m_Animator.SetBool("Attack", false);
        npcManager.SwitchState(npcManager.PursueState);
    }
}
