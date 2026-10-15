using Verse;
using UnityEngine;

namespace IntellectualMatters
{
    public class IntellectualMatters_Settings : ModSettings
    {
        // Curve that scales GlobalLearningFactor by Intellectual level
        public float factor0 = 0.80f;
        public float factor5 = 0.95f;
        public float factor10 = 1.10f;
        public float factor15 = 1.25f;
        public float factor20 = 1.40f;

        // Multiplier for Intellectual skill decay only (0..3)
        public float intellectualDecayMult = 1.0f;

        // At local midnight: grant Intellectual XP = sum(other skills' xpSinceMidnight) * this
        // (0..3 => 0%..300%), default 0.05 (5%)
        public float otherSkillsToIntellectualFactor = 0.05f;

        // When true, midnight grants write per-skill lines to the log
        public bool debugLogging = false;

        public static IntellectualMatters_Settings Current;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref factor0, "factor0", 0.80f);
            Scribe_Values.Look(ref factor5, "factor5", 0.95f);
            Scribe_Values.Look(ref factor10, "factor10", 1.10f);
            Scribe_Values.Look(ref factor15, "factor15", 1.25f);
            Scribe_Values.Look(ref factor20, "factor20", 1.40f);

            Scribe_Values.Look(ref intellectualDecayMult, "intellectualDecayMult", 1.0f);
            Scribe_Values.Look(ref otherSkillsToIntellectualFactor, "otherSkillsToIntellectualFactor", 0.05f);
            Scribe_Values.Look(ref debugLogging, "debugLogging", false);
        }

        public void EnforceBoundsAndMonotone()
        {
            factor0 = Mathf.Clamp(factor0, 0.1f, 3f);
            factor5 = Mathf.Clamp(factor5, 0.1f, 3f);
            factor10 = Mathf.Clamp(factor10, 0.1f, 3f);
            factor15 = Mathf.Clamp(factor15, 0.1f, 3f);
            factor20 = Mathf.Clamp(factor20, 0.1f, 3f);

            if (factor5 < factor0) factor5 = factor0;
            if (factor10 < factor5) factor10 = factor5;
            if (factor15 < factor10) factor15 = factor10;
            if (factor20 < factor15) factor20 = factor15;

            intellectualDecayMult = Mathf.Clamp(intellectualDecayMult, 0f, 3f);
            otherSkillsToIntellectualFactor = Mathf.Clamp(otherSkillsToIntellectualFactor, 0f, 3f);
        }

        public float GetFactorForSkill(int lvl)
        {
            if (lvl <= 0) return factor0;
            if (lvl <= 5) return Mathf.Lerp(factor0, factor5, lvl / 5f);
            if (lvl <= 10) return Mathf.Lerp(factor5, factor10, (lvl - 5) / 5f);
            if (lvl <= 15) return Mathf.Lerp(factor10, factor15, (lvl - 10) / 5f);
            if (lvl <= 20) return Mathf.Lerp(factor15, factor20, (lvl - 15) / 5f);
            return factor20;
        }

        public static void DebugLog(string message)
        {
            if (Current == null || !Current.debugLogging) return;
            Log.Message(message);
        }
    }
}
