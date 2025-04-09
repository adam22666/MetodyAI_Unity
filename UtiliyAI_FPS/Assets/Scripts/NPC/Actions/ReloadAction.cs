using UnityEngine;
using System.Collections;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Reload", menuName = "UtilityAI/Actions/Reload")]
    public class ReloadAction : Action
    {
        // Prepisem aj ParallelExecute, aby reload prebehol aj pocas pohybu
        public override void ParallelExecute(NPCController npc)
        {
            if (!npc.isReloading)
            {
                npc.isReloading = true;
                npc.StartCoroutine(Reloading(npc));
            }
        }

        public override void Execute(NPCController npc)
        {
            npc.AIBrain.finishedExecutingBestAction = true;
        }

        private IEnumerator Reloading(NPCController npc)
        {
            yield return new WaitForSeconds(2f);
            npc.Stats.ammo = npc.Stats.maxAmmo;
            Debug.Log("Reload hotový, ammo na maximum!");
            npc.isReloading = false;
            npc.AIBrain.finishedExecutingBestAction = true;
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            RequiredDestination = npc.transform;
        }
    }
}