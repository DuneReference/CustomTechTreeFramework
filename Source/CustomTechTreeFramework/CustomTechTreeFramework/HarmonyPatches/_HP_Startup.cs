using Verse;
using HarmonyLib;

namespace DuneRef_CustomTechTreeFramework
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static Harmony Harm;

        static HarmonyPatches()
        {
            Harm = new Harmony("rimworld.mod.duneref.customtechtreeframework");

            if (ModLister.HasActiveModWithName("ResearchPowl"))
            {
                ResearchPowlPatches.Patches();
            }
            else 
            if (ModLister.HasActiveModWithName("Dubs Mint Menus"))
            {
                DubsMintMenusPatches.Patches();
            }
            else
            {
                VanillaPatches.ExclusivePatches();
            }

            if (ModLister.HasActiveModWithName("Semi Random Research (Unofficial)"))
            {
                SemiRandomResearchPatches.Patches();
            }

            if (ModLister.HasActiveModWithName("Combat Extended"))
            {
                if (ModLister.HasActiveModWithName("Vanilla Expanded Framework"))
                {
                    CombatExtendedPatches.Patches();
                } else
                {
                    Log.Message("CE Recipe Integration Patch skipped because Vanilla Expanded Framework is required and was not found.");
                }
            }

            if (CustomTechTreeFrameworkSettings.removeCostFactor)
            {
                VanillaPatches.CostFactorPatch();
            }

            VanillaPatches.Patches();
        }
    }
}