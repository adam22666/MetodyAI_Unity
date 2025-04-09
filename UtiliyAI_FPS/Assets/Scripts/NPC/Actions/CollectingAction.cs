using UnityEngine;
using TL.UtilityAI;
using TL.Core;
using System.Collections;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Collecting", menuName = "UtilityAI/Actions/Collecting")]
    public class CollectingAction : Action
    {
        [SerializeField] private float collectDuration = 1f; // cas na zbieranie resource
        [SerializeField] private float collectionThreshold = 1.0f; // Tolerancia vzdialenosti (v metroch)
        [SerializeField] private float collectionTimeout = 5f; // Max cas cakania na priblizenie sa k resource

        private Transform targetResource; 

        public override void SetRequiredDestination(NPCController npc)
        {
            GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
            if (resources.Length == 0)
            {
                RequiredDestination = npc.transform;
                Debug.Log("CollectingAction: ziadne resource s tagom 'Resource' nenajdene.");
                return;
            }

            float nearestDistance = Mathf.Infinity;
            Transform bestResource = null;
            foreach (GameObject res in resources)
            {
                float d = Vector3.Distance(npc.transform.position, res.transform.position);
                if (d < nearestDistance)
                {
                    nearestDistance = d;
                    bestResource = res.transform;
                }
            }
            if (bestResource != null)
            {
                targetResource = bestResource;
                // NPC aby zastavilo 0,5 m od resource:
                Vector3 direction = (npc.transform.position - bestResource.position).normalized;
                if (direction == Vector3.zero)
                    direction = npc.transform.forward;
                Vector3 destination = bestResource.position + direction * 0.5f;
                npc.Agent.stoppingDistance = 0.5f;
                npc.Agent.SetDestination(destination);
                RequiredDestination = bestResource;
                Debug.Log("CollectingAction: Nastavený cieľ na resource s offsetom 0.5 m.");
            }
            else
            {
                // keby sa to nestalo
                RequiredDestination = npc.transform;
            }
        }

        public override void Execute(NPCController npc)
        {
            npc.StartCoroutine(CollectResource(npc));
        }

        private IEnumerator CollectResource(NPCController npc)
        {
            if (targetResource == null)
            {
                Debug.Log("CollectingAction: Resource nebol najdeny.");
                npc.AIBrain.finishedExecutingBestAction = true;
                yield break;
            }

            float timer = 0f;
            // Čakame, kym NPC sa priblizi do collectionThreshold alebo kym neuplynie timeout
            while (targetResource != null && Vector3.Distance(npc.transform.position, targetResource.position) > collectionThreshold && timer < collectionTimeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            if (targetResource == null)
            {
                npc.AIBrain.finishedExecutingBestAction = true;
                yield break;
            }
            
            Debug.Log("CollectingAction: NPC je blizko resource, zacnam zbierat...");
            yield return new WaitForSeconds(collectDuration);
            
            if (targetResource != null)
            {
                npc.Stats.grenades++;
                Debug.Log("CollectingAction: Resource zozbierany, granatov: " + npc.Stats.grenades);
                Destroy(targetResource.gameObject);
            }
            
            
            npc.AIBrain.finishedExecutingBestAction = true;
            npc.currentState = NPCController.State.Decide;
        }

        public override void ParallelExecute(NPCController npc)
        {
            Execute(npc);
        }
    }
}
