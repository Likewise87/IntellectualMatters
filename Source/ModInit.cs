using System;
using UnityEngine;
using Verse;

namespace IntellectualMatters
{
    public enum SliderFormat { Fixed0, Fixed1, Fixed2, Multiplier, Percent, PercentDecimal }

    public class ModInit : Mod
    {
        private IntellectualMatters_Settings settings;
        private Vector2 scrollPos = Vector2.zero;

        private const float SliderMin = 0.1f;
        private const float SliderMax = 3.0f;

        public ModInit(ModContentPack content) : base(content)
        {
            settings = GetSettings<IntellectualMatters_Settings>();
            IntellectualMatters_Settings.Current = settings;
            Bootstrap.Init();
        }

        public override string SettingsCategory() => "Intellectual Matters";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var s = settings;
            float contentWidth = inRect.width - 24f;
            Rect viewRect = new Rect(0f, 0f, contentWidth, 480f);

            Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
            var l = new Listing_Standard();
            l.Begin(viewRect);

            DrawResetButton(l, () =>
            {
                s.factor0 = 0.80f;
                s.factor5 = 0.95f;
                s.factor10 = 1.10f;
                s.factor15 = 1.25f;
                s.factor20 = 1.40f;
                s.intellectualDecayMult = 1.0f;
                s.otherSkillsToIntellectualFactor = 0.05f;
                s.debugLogging = false;
                s.EnforceBoundsAndMonotone();
            });

            l.Label("Intellectual scales global learning speed. Training XP from other skills is granted once per day at local midnight (not continuously while working).");
            l.Gap(6f);

            l.Label("Global learning factor by Intellectual level");
            l.Gap(4f);

            s.factor0 = LabeledSlider(l, "Intellectual level 0", s.factor0, SliderMin, SliderMax,
                "Global learning factor multiplier at Intellectual level 0.", 0.01f, SliderFormat.Multiplier, 0.80f);
            s.factor5 = LabeledSlider(l, "Intellectual level 5", s.factor5, SliderMin, SliderMax,
                "Global learning factor multiplier at Intellectual level 5.", 0.01f, SliderFormat.Multiplier, 0.95f);
            s.factor10 = LabeledSlider(l, "Intellectual level 10", s.factor10, SliderMin, SliderMax,
                "Global learning factor multiplier at Intellectual level 10.", 0.01f, SliderFormat.Multiplier, 1.10f);
            s.factor15 = LabeledSlider(l, "Intellectual level 15", s.factor15, SliderMin, SliderMax,
                "Global learning factor multiplier at Intellectual level 15.", 0.01f, SliderFormat.Multiplier, 1.25f);
            s.factor20 = LabeledSlider(l, "Intellectual level 20", s.factor20, SliderMin, SliderMax,
                "Global learning factor multiplier at Intellectual level 20.", 0.01f, SliderFormat.Multiplier, 1.40f);

            l.Gap(8f);
            l.Label("Daily Intellectual training");
            l.Gap(4f);

            s.otherSkillsToIntellectualFactor = LabeledSlider(l, "Daily XP from other skills", s.otherSkillsToIntellectualFactor, 0f, 3f,
                "At local midnight, each humanlike pawn gains Intellectual XP equal to this percentage of all XP they earned that day in every other skill (RimWorld's xpSinceMidnight totals). Does not grant XP while working.",
                0.01f, SliderFormat.Percent, 0.05f);

            l.Gap(8f);
            l.Label("Intellectual skill decay");
            l.Gap(4f);

            s.intellectualDecayMult = LabeledSlider(l, "Decay rate", s.intellectualDecayMult, 0f, 3f,
                "Multiplier for Intellectual skill XP loss from disuse. Does not affect the daily training grant.",
                0.01f, SliderFormat.Percent, 1.0f);

            l.Gap(8f);
            l.CheckboxLabeled("Debug Logging", ref s.debugLogging,
                "When enabled, logs each contributing skill's midnight Intellectual XP grant to the debug log.");

            l.End();
            Widgets.EndScrollView();

            s.EnforceBoundsAndMonotone();
        }

        private static void DrawResetButton(Listing_Standard l, Action onReset)
        {
            if (onReset == null) return;
            l.Gap(4f);
            Rect row = l.GetRect(30f);
            float resetW = Mathf.Min(250f, row.width);
            if (resetW < 120f) resetW = Mathf.Max(100f, row.width * 0.35f);
            Rect resetRect = new Rect(row.x, row.y, resetW, row.height);
            Color prev = GUI.color;
            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (Widgets.ButtonText(resetRect, "Restore defaults")) onReset();
            GUI.color = prev;
            l.Gap(6f);
        }

        private static float LabeledSlider(Listing_Standard l, string label, float val, float min, float max, string tooltip = null, float step = 0.1f, SliderFormat format = SliderFormat.Fixed1, float? defaultValue = null)
        {
            l.Gap(2f);
            Rect r = l.GetRect(24f);
            if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(r, TooltipWithDefault(tooltip, defaultValue, format));
            Widgets.Label(r.LeftPart(0.5f), $"{label}: {FormatValue(val, format).Colorize(Color.cyan)}");
            return Widgets.HorizontalSlider(r.RightPart(0.5f), val, min, max, false, null, null, null, step);
        }

        private static string TooltipWithDefault(string tooltip, float? defaultValue, SliderFormat format = SliderFormat.Fixed1)
        {
            if (!defaultValue.HasValue) return tooltip;
            return AppendDefault(tooltip, FormatValue(defaultValue.Value, format));
        }

        private static string AppendDefault(string tooltip, string def)
        {
            string line = $"Default: {def}";
            return tooltip.NullOrEmpty() ? line : tooltip + "\n" + line;
        }

        private static string FormatValue(float val, SliderFormat format) => format switch
        {
            SliderFormat.Fixed0 => val.ToString("F0"),
            SliderFormat.Fixed1 => val.ToString("F1"),
            SliderFormat.Fixed2 => val.ToString("F2"),
            SliderFormat.Multiplier => val.ToString("F2") + "x",
            SliderFormat.Percent => (val * 100f).ToString("F0") + " %",
            SliderFormat.PercentDecimal => (val * 100f).ToString("F1") + " %",
            _ => val.ToString()
        };
    }
}
