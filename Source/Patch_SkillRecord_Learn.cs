using HarmonyLib;
using RimWorld;
using Verse;

namespace IntellectualMatters
{
    // Scales Intellectual skill decay only. Positive XP (research, midnight grant, etc.) is unchanged.
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
    public static class Patch_SkillRecord_Learn
    {
        public static void Prefix(SkillRecord __instance, ref float xp)
        {
            if (__instance.def != SkillDefOf.Intellectual) return;
            if (xp >= 0f) return;

            var s = IntellectualMatters_Settings.Current;
            if (s == null) return;

            xp *= s.intellectualDecayMult;
        }
    }
}
