using UnityEngine;
using TL.Core;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Distance_Shoot", menuName = "UtilityAI/Considerations/Distance_Shoot")]
    public class ShootDistanceConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        [SerializeField] private float maxDistance = 25f;  // Max vzdialenost, pri ktorej je strelba uplne preferovana

        public override float ScoreConsideration(NPCController npc)
        {
            if (Context.Instance == null || Context.Instance.Player == null)
                return 0f;
            
            float distance = Vector3.Distance(npc.transform.position, Context.Instance.Player.position);
            float normalized = Mathf.Clamp01(distance / maxDistance); // 0 pri 0 m, 1 pri maxDistance
            float baseScore = responseCurve.Evaluate(normalized);
            
            score = baseScore;
            return baseScore;
        }
    }
}
