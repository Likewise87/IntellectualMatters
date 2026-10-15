using HarmonyLib;
using RimWorld;
using Verse;

namespace IntellectualMatters
{
    // Once per local midnight (same moment vanilla clears xpSinceMidnight),
    // grant Intellectual XP = sum(other skills' XP today) * factor.
    [HarmonyPatch(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.SkillsTickInterval))]
    public static class Patch_SkillsTickInterval_DailyIntellectual
    {
        private static readonly AccessTools.FieldRef<Pawn_SkillTracker, Pawn> PawnRef =
            AccessTools.FieldRefAccess<Pawn_SkillTracker, Pawn>("pawn");

        private static readonly AccessTools.FieldRef<Pawn_SkillTracker, int> LastResetRef =
            AccessTools.FieldRefAccess<Pawn_SkillTracker, int>("lastXpSinceMidnightResetTimestamp");

        public static void Prefix(Pawn_SkillTracker __instance, int delta)
        {
            var settings = IntellectualMatters_Settings.Current;
            if (settings == null) return;

            float factor = settings.otherSkillsToIntellectualFactor;
            if (factor <= 0f) return;

            Pawn pawn = PawnRef(__instance);
            if (pawn?.skills == null) return;
            if (pawn.RaceProps == null || !pawn.RaceProps.Humanlike) return;
            if (!pawn.IsHashIntervalTick(200, delta)) return;
            if (!CanGainXP(pawn)) return;

            // Mirror vanilla midnight-reset gate in SkillsTickInterval.
            int lastReset = LastResetRef(__instance);
            if (GenLocalDate.HourInteger(pawn) != 0) return;
            if (lastReset >= 0 && Find.TickManager.TicksGame - lastReset < 30000) return;

            float bonus = 0f;
            var skills = __instance.skills;
            for (int i = 0; i < skills.Count; i++)
            {
                SkillRecord sk = skills[i];
                if (sk.def == SkillDefOf.Intellectual) continue;
                float granted = sk.xpSinceMidnight * factor;
                if (granted <= 0f) continue;
                bonus += granted;
                IntellectualMatters_Settings.DebugLog(
                    $"[IntellectualMatters] {pawn.LabelShortCap} {sk.def.defName} -> Intellectual +{granted:0.##} XP (source {sk.xpSinceMidnight:0.##} × {factor:0.##})");
            }

            if (bonus <= 0f) return;

            __instance.GetSkill(SkillDefOf.Intellectual).Learn(bonus, direct: true);
        }

        private static bool CanGainXP(Pawn pawn)
        {
            if (ModsConfig.AnomalyActive && pawn.IsMutant && pawn.mutant?.Def != null && !pawn.mutant.Def.canGainXP)
                return false;
            return true;
        }
    }
}
