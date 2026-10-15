using HarmonyLib;
using Verse;

namespace IntellectualMatters
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            Init();
        }

        public static void Init()
        {
            var h = new Harmony("TSA.IntellectualMatters");
            h.PatchAll();
        }
    }
}
