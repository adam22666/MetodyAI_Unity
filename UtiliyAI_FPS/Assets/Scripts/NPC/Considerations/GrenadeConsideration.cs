using UnityEngine;
using TL.Core;
using TL.UtilityAI;

[CreateAssetMenu(fileName = "GrenadeConsideration", menuName = "UtilityAI/Considerations/GrenadeConsideration")]
public class GrenadeConsideration : Consideration
{
    [Header("Thresholds")]
    [Range(0f, 1f)]
    public float healthThreshold = 0.3f;
    
    public float distanceThreshold = 10f;
    
    [Header("Response Curves")]
    public AnimationCurve healthCurve;
    
    public AnimationCurve distanceCurve;
    
    [Header("Multiplier")]
    public float multiplier = 2f;
    
    public override float ScoreConsideration(NPCController npc)
    {
        if (npc == null || npc.Stats == null || Context.Instance?.Player == null)
            return 0f;
        
        // Ak NPC nema granaty, vratime 0
        if (npc.Stats.grenades <= 0)
        {
            score = 0f;
            return 0f;
        }
        
        float dist = Vector3.Distance(npc.transform.position, Context.Instance.Player.position);
        if (dist > distanceThreshold)
        {
            score = 0f;
            return 0f;
        }
        
        float healthPercent = (float)npc.Stats.health / npc.Stats.maxHealth;
        // Ak je HP pod threshold, deficit = (threshold - current)/threshold; inak deficit = 0.
        float healthDeficit = healthPercent < healthThreshold ? (healthThreshold - healthPercent) / healthThreshold : 0f;
        
        float healthScore = healthCurve.Evaluate(healthDeficit);
        float distanceFactor = (distanceThreshold - dist) / distanceThreshold; // 1, ked je hrac uplne blizko, klesa k 0 pri distanceThreshold
        distanceFactor = Mathf.Clamp01(distanceFactor);
        float distanceScore = distanceCurve.Evaluate(distanceFactor);
        
        float baseScore = healthScore * distanceScore;
        float finalScore = multiplier * baseScore;
        score = finalScore;
        return finalScore;
    }
}
