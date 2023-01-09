using System;

using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace DuneRef_CustomTechTreeFramework
{
    public static class VanillaPatches
    {
        public static readonly Type patchType = typeof(VanillaPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void ExclusivePatches()
        {
            // Finish linked projects
            Harm.Patch(AccessTools.Method(typeof(ResearchManager), "FinishProject"), postfix: new HarmonyMethod(CommonPatches.patchType, nameof(CommonPatches.FinishProjectOmniFix)));
        }

        public static void Patches()
        {
            // Show icons of linked projects for my projects 
            Harm.Patch(AccessTools.PropertyGetter(typeof(ResearchProjectDef), nameof(ResearchProjectDef.UnlockedDefs)), prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(UnlockedDefsPrefix)));

            // Hide projects that I designate for hiding.
            Harm.Patch(AccessTools.Method(typeof(MainTabWindow_Research), nameof(MainTabWindow_Research.PostOpen)), postfix: new HarmonyMethod(patchType, nameof(PostOpenPostfix)));

            // Remove CostFactor adjustments
            Harm.Patch(AccessTools.Method(typeof(ResearchProjectDef), nameof(ResearchProjectDef.CostFactor)), postfix: new HarmonyMethod(patchType, nameof(CostFactorPostfix)));
        }

        public static bool UnlockedDefsPrefix(ref List<Def> __result, ResearchProjectDef __instance)
        {
            if (__instance.cachedUnlockedDefs == null)
            {
                __instance.cachedUnlockedDefs = Utility.GetUnlocksForResearch(__instance);
            }

            __result = __instance.cachedUnlockedDefs;

            return false;
        }

        public static void PostOpenPostfix(MainTabWindow_Research __instance)
        {
            __instance.tabs.Clear();

            foreach (ResearchTabDef tabDef in DefDatabase<ResearchTabDef>.AllDefs)
            {
                if (tabDef.GetModExtension<HiddenTab>() == null || tabDef.GetModExtension<HiddenTab>().hidden == false)
                {
                    __instance.tabs.Add(new MainTabWindow_Research.ResearchTabRecord(tabDef, tabDef.LabelCap, () => __instance.CurTab = tabDef, () => __instance.CurTab == tabDef));
                }
            }
        }

        public static void CostFactorPostfix(ref float __result)
        {
            __result = 1f;
        }
    }
}