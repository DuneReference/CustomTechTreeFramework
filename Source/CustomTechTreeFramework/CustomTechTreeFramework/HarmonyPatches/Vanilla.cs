using System;

using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace DuneRef_CustomTechTreeFramework
{
    public static class VanillaPatches
    {
        public static readonly Type PatchType = typeof(VanillaPatches);
        public static Harmony Harm = HarmonyPatches.Harm;
        public static bool CostFactorPatched = false;

        public static void ExclusivePatches()
        {
            // Finish linked projects
            Harm.Patch(AccessTools.Method(typeof(ResearchManager), "FinishProject"), postfix: new HarmonyMethod(CommonPatches.PatchType, nameof(CommonPatches.FinishProjectOmniFix)));
        }

        public static void Patches()
        {
            // Show icons of linked projects for my projects 
            Harm.Patch(AccessTools.PropertyGetter(typeof(ResearchProjectDef), nameof(ResearchProjectDef.UnlockedDefs)), prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(UnlockedDefsPrefix)));

            // Hide projects that I designate for hiding.
            Harm.Patch(AccessTools.Method(typeof(MainTabWindow_Research), nameof(MainTabWindow_Research.PostOpen)), postfix: new HarmonyMethod(PatchType, nameof(PostOpenPostfix)));

            // Add functionality for OR Prerequisites
            Harm.Patch(AccessTools.PropertyGetter(typeof(ResearchProjectDef), nameof(ResearchProjectDef.PrerequisitesCompleted)), postfix: new HarmonyMethod(PatchType, nameof(AddOrPrerequisites)));
        }

        public static void CostFactorPatch()
        {
            // Remove CostFactor adjustments 
            Harm.Patch(AccessTools.Method(typeof(ResearchProjectDef), nameof(ResearchProjectDef.CostFactor)), postfix: new HarmonyMethod(PatchType, nameof(CostFactorPostfix)));
            CostFactorPatched = true;
        }

        public static void CostFactorUnpatch()
        {
            Harm.Unpatch(AccessTools.Method(typeof(ResearchProjectDef), nameof(ResearchProjectDef.CostFactor)), AccessTools.Method(PatchType, nameof(CostFactorPostfix)));
            CostFactorPatched = false;
        }

        public static void UpdateCostFactorPatch()
        {
            if (CostFactorPatched && !CustomTechTreeFrameworkSettings.RemoveCostFactor)
            {
                CostFactorUnpatch();
            }
            else if (!CostFactorPatched && CustomTechTreeFrameworkSettings.RemoveCostFactor)
            {
                CostFactorPatch();
            }
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

            foreach (var tabDef in DefDatabase<ResearchTabDef>.AllDefs)
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

        public static void AddOrPrerequisites(ref bool __result, ResearchProjectDef __instance)
        {
            if (__result)
                return;

            var orPrerequisitesModExtension = __instance.GetModExtension<OrPrerequisites>();
            var orPrerequisites = orPrerequisitesModExtension?.orPrerequisites;
            var orHiddenPrerequisites = orPrerequisitesModExtension?.orHiddenPrerequisites;

            var anyFinished = false;

            if (orPrerequisites?.Count > 0)
            {
                foreach(var orPrerequisite in orPrerequisites)
                {
                    if (orPrerequisite.IsFinished)
                    {
                        anyFinished = true;
                        break;
                    }
                }

                if (!anyFinished)
                {
                    __result = false;
                    return;
                }
            }

            if (orHiddenPrerequisites == null || orHiddenPrerequisites.Count <= 0)
                return;

            anyFinished = false;

            foreach (var orHiddenPrerequisite in orHiddenPrerequisites)
            {
                if (orHiddenPrerequisite.IsFinished)
                {
                    anyFinished = true;
                    break;
                }
            }

            if (!anyFinished)
            {
                __result = false;
            }
        }
    }
}