using UnityEngine;
public class NPCAttackState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Attack;
        npcManager._agent.isStopped = true;
        npcManager._agent.ResetPath();
        npcManager._animator.SetTrigger("Attacking");
    }
    public override void UpdateState(NPCManagerScript npcManager)
    {
        if (!npcManager.InTargetProximity())
        {
            npcManager._agent.ResetPath();
            ExitState(npcManager);
        }
    }

    public override void ExitState(NPCManagerScript npcManager)
    {
        npcManager._animator.ResetTrigger("Attacking");
        npcManager.SwitchState(npcManager.PursueState);
    }
}
