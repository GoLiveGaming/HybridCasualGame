using UnityEngine;
using UnityEngine.AI;
public abstract class NPCBaseState
{
    public abstract void EnterState(NPCManagerScript npcManager);
    public abstract void UpdateState(NPCManagerScript npcManager);
    public abstract void ExitState(NPCManagerScript npcManager);

}
