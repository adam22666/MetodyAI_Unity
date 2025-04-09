using UnityEngine;
using TL.UtilityAI;
using TL.Core;
using System.Collections;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Grenade", menuName = "UtilityAI/Actions/Grenade")]
    public class GrenadeAction : Action
    {
        [SerializeField] private float grenadeCooldown = 2.5f; 
        
        public override void ParallelExecute(NPCController npc)
        {
            Execute(npc);
        }
        
        public override void Execute(NPCController npc)
        {
            // NPC cooldown
            if (Time.time - npc.lastGrenadeTimeGrenade >= grenadeCooldown)
            {
                if (npc.Stats.grenades > 0)
                {
                    npc.ThrowGrenade();
                    npc.lastGrenadeTimeGrenade = Time.time;
                }
                else
                {
                    Debug.Log("GrenadeAction: ziadne granaty!");
                }
                npc.AIBrain.finishedExecutingBestAction = true;
            }
            else
            {
                npc.AIBrain.finishedExecutingBestAction = false;
            }
        }
        
        public override void SetRequiredDestination(NPCController npc)
        {
            if (Context.Instance != null && Context.Instance.Player != null)
            {
                RequiredDestination = Context.Instance.Player;
            }
        }
    }
}
