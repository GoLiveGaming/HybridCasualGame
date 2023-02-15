using UnityEngine;
public class NPCPursueState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Pursue;
        npcManager.UpdateDestination();
        npcManager._animator.SetTrigger("Running");
    }
    public override void UpdateState(NPCManagerScript npcManager)
    {
        if (npcManager.InTargetProximity())
        {
            npcManager._agent.ResetPath();
            //  npcManager._agent.isStopped = true;
            ExitState(npcManager);
        }
        else
        {
            npcManager.UpdateDestination();
        }
    }
    public override void ExitState(NPCManagerScript npcManager)
    {
        npcManager._animator.ResetTrigger("Running");
        npcManager.SwitchState(npcManager.AttackState);
    }

}
