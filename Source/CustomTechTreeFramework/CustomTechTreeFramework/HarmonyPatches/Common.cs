using System;

using Verse;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using DubsMintMenus;

namespace DuneRef_CustomTechTreeFramework
{
    public static class CommonPatches
    {
        public static readonly Type patchType = typeof(CommonPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void FinishProjectOmniFix(ResearchProjectDef proj)
        {
            if (!proj.IsFinished)
                return;

            if (proj.GetModExtension<ResearchUnlocks>() != null)
            {
                foreach (ResearchProjectDef unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                {
                    List<ResearchProjectDef> researchDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                                                            .Where(x => x.tab != RimCivTechTree_DefOf.DuneRef_Hidden)
                                                            .ToList();

                    bool notFinished = false;

                    foreach (ResearchProjectDef researchDef in researchDefs)
                    {
                        if (researchDef != proj &&
                            !researchDef.IsFinished &&
                            researchDef.GetModExtension<ResearchUnlocks>() != null &&
                            researchDef.GetModExtension<ResearchUnlocks>().researchUnlocks.Contains(unlock))
                        {
                            notFinished = true;
                        }
                    }

                    if (!notFinished)
                    {
                        Find.ResearchManager.FinishProject(unlock);
                    }
                }
            }
        }
    }
}