using UnityEngine;
public class NPCPursueState : NPCBaseState
{
    public override void EnterState(NPCManagerScript npcManager)
    {
        npcManager.activeState = NPCManagerScript.NPCStates.Pursue;
    }
    public override void UpdateState(NPCManagerScript npcManager)
    {

        if (npcManager.InTargetProximity())
        {
            npcManager._agent.ResetPath();
            ExitState(npcManager);
        }
        else
        {
            npcManager.UpdateDestination();
        }
       
    }
    public override void ExitState(NPCManagerScript npcManager)
    {
        npcManager.SwitchState(npcManager.AttackState);
    }

}
