using UnityEngine;
public class NPCPursueState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Pursue;
        npcManager._agent.isStopped = true;
        npcManager._agent.ResetPath();
        npcManager.UpdateDestination();
        npcManager._animator.SetTrigger("Running");
    }
    public override void UpdateState(NPCManagerScript npcManager)
    {
        if (npcManager.InTargetProximity())
        {
            npcManager._agent.ResetPath();
            ExitState(npcManager);
        }
    }
    public override void ExitState(NPCManagerScript npcManager)
    {
        npcManager._animator.ResetTrigger("Running");
        npcManager.SwitchState(npcManager.AttackState);
    }

}
