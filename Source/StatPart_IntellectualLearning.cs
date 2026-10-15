using RimWorld;
using Verse;

namespace IntellectualMatters
{
    public class StatPart_IntellectualLearning : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing || req.Thing is not Pawn pawn) return;
            if (pawn.skills == null) return;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Intellectual).Level;
            float factor = IntellectualMatters_Settings.Current.GetFactorForSkill(lvl);

            val *= factor;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || req.Thing is not Pawn pawn) return null;
            if (pawn.skills == null) return null;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Intellectual).Level;
            float skillFactor = IntellectualMatters_Settings.Current.GetFactorForSkill(lvl);

            float finalLearning = pawn.GetStatValue(StatDefOf.GlobalLearningFactor, true);

            return $"Skill→Learning (Intellectual {lvl}/20): x{skillFactor * 100:0}%\n" +
                   $"→ Final learning factor: x{finalLearning * 100:0}%";
        }
    }
}
