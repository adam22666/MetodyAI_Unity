using UnityEngine;
using System.Collections;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Shoot", menuName = "UtilityAI/Actions/Shoot")]
    public class ShootAction : Action
    {
        [SerializeField] private int damagePerShot = 25;
        [SerializeField] private float minPlayerDistance = 2f;

        public override void Execute(NPCController npc)
        {
            npc.AIBrain.finishedExecutingBestAction = true;
        }

        //  Metodda ktora sa vola v stave Move, aby NPC mohla strielat popri behu
        public override void ParallelExecute(NPCController npc)
        {
            if (npc.Stats.ammo > 0)
            {
                // npc.Shoot() => odcita 1 naboj + spawn strely + cooldown
                npc.Shoot();
                ApplyDamageToPlayer(npc);
            }
            else
            {
                npc.AIBrain.finishedExecutingBestAction = true;
            }
        }

        private void ApplyDamageToPlayer(NPCController npc)
        {
            if (Context.Instance.Player != null)
            {
                Health playerHealth = Context.Instance.Player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerShot);
                }
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            if (Context.Instance != null && Context.Instance.Player != null)
            {
                RequiredDestination = Context.Instance.Player;
                npc.Agent.destination = RequiredDestination.position;
                npc.Agent.stoppingDistance = minPlayerDistance;
            }
        }
    }
}
