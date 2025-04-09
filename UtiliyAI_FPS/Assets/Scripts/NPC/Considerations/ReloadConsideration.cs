using UnityEngine;
using TL.Core;
using TL.UtilityAI;

[CreateAssetMenu(fileName = "ReloadConsideration", menuName = "UtilityAI/Considerations/ReloadConsideration")]
public class ReloadConsideration : Consideration
{
    [Header("Settings")]
    public float farDistanceThreshold = 20f;

    [Range(0f, 1f)]
    public float minHealthForReload = 0.5f;

    [Header("Distance Settings")]
    public float nearDistanceThreshold = 10f;

    [Header("Response Curve")]
    public AnimationCurve ammoCurve;

    public override float ScoreConsideration(NPCController npc)
    {
        if (npc == null || npc.Stats == null || Context.Instance?.Player == null)
            return 0f;

        // Ak ammo je 0, reload je urgent.
        if (npc.Stats.ammo <= 0)
        {
            score = 1f;
            return score;
        }

        // Ak ma NPC menej ako 50% HP, reload sa nevykona (vracia 0)
        float healthPercent = (float)npc.Stats.health / npc.Stats.maxHealth;
        if (healthPercent < minHealthForReload)
        {
            score = 0f;
            return score;
        }

        // vzidalenost od hraca
        float dist = Vector3.Distance(npc.transform.position, Context.Instance.Player.position);

        // ak je player moc blizko, reload sa nevykona
        if (dist <= nearDistanceThreshold)
        {
            score = 0f;
            return score;
        }

        // Ak je playew dalej ako farDistanceThreshold, reload je preferovany
        if (dist >= farDistanceThreshold)
        {
            score = 1f;
            return score;
        }

        // Inak plynule interpolujeme medzi nearDistanceThreshold a farDistanceThreshold.
        float t = Mathf.InverseLerp(nearDistanceThreshold, farDistanceThreshold, dist);
        // Vypocet ammo deficit
        float ammoPercent = (float)npc.Stats.ammo / npc.Stats.maxAmmo;
        float ammoDeficit = 1f - ammoPercent;
        //  utility z ammoCurve.
        float ammoScore = ammoCurve.Evaluate(ammoDeficit);
        // Interpolovaná hodnota – cim dalej je player, tym viac sa utility bliži k ammoScore (max 1).
        float finalScore = Mathf.Lerp(0f, ammoScore, t);
        score = finalScore;
        return finalScore;
    }
}
