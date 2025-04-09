using UnityEngine;
using TL.Core;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Ammo", menuName = "UtilityAI/Considerations/Ammo")]
    public class AmmoConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        
        public override float ScoreConsideration(NPCController npc)
        {
            // Ak je ammo naozaj 0, vratime 0
            if (npc.Stats.ammo <= 0)
            {
                score = 0f;
                Debug.Log($"AmmoConsideration: ammo=0, score=0");
                return score;
            }
            
            float ammoPercent = (float)npc.Stats.ammo / npc.Stats.maxAmmo;
            float param = 1f - ammoPercent; 
            score = responseCurve.Evaluate(param);
            Debug.Log($"AmmoConsideration: ammoPercent={ammoPercent:F2}, param={param:F2}, score={score:F2}");
            return score;
        }
    }
}
