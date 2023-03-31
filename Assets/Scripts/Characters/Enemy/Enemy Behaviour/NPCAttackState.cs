using UnityEngine;
public class NPCAttackState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Attack;
        npcManager._agent.isStopped = true;
        npcManager._agent.ResetPath();
        npcManager._animator.SetBool("Attack", true);
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
        npcManager._animator.SetBool("Attack", false);
        npcManager.SwitchState(npcManager.PursueState);
    }
}
