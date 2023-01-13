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

            if (CustomTechTreeFrameworkSettings.removeCostFactor)
            {
                VanillaPatches.CostFactorPatch();
            }

            VanillaPatches.Patches();
        }
    }
}