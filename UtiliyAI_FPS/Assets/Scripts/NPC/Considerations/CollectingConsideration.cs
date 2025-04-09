using UnityEngine;
using TL.Core;
using TL.UtilityAI;

[CreateAssetMenu(fileName = "CollectingConsideration", menuName = "UtilityAI/Considerations/CollectingConsideration")]
public class CollectingConsideration : Consideration
{
    [Header("Settings")]
    public bool requireZeroGrenades = true;
    
    [Header("Resource Distance Settings")]
    public float maxResourceDistance = 10f;
    
    [Header("Multiplier")]
    public float multiplier = 1f;
    
    public override float ScoreConsideration(NPCController npc)
    {
        if(npc == null || npc.Stats == null || Context.Instance?.Player == null)
            return 0f;
        
        // Ak NPC ma spon 1 granat, collecting sa nevykona.
        if (requireZeroGrenades && npc.Stats.grenades > 0)
        {
            score = 0f;
            return 0f;
        }
        
        // najdi najblizsi resource
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        float resourceUtility = 0f;
        if (resources.Length > 0)
        {
            float minResourceDist = float.MaxValue;
            foreach (GameObject res in resources)
            {
                float d = Vector3.Distance(npc.transform.position, res.transform.position);
                if (d < minResourceDist)
                    minResourceDist = d;
            }
            // Ak je resource uplne vedaa NPC (minResourceDist = 0), faktor = 1; ak je resource na maxResourceDistance alebo dalej, faktor = 0.
            resourceUtility = Mathf.Clamp01((maxResourceDistance - minResourceDist) / maxResourceDistance);
        }
        
        float finalScore = resourceUtility * multiplier;
        score = finalScore;
        return finalScore;
    }
}
