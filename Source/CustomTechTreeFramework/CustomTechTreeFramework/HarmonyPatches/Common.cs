using System;

using Verse;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;

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

            ResearchUnlocks projResearchUnlocks = proj.GetModExtension<ResearchUnlocks>();

            if (projResearchUnlocks != null)
            {
                foreach (ResearchProjectDef unlock in projResearchUnlocks.researchUnlocks)
                {
                    List<ResearchProjectDef> researchDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                                                            .Where(x => x.tab != CustomTechTreeFramework_DefOf.DuneRef_Hidden)
                                                            .ToList();

                    bool notFinished = false;

                    foreach (ResearchProjectDef researchDef in researchDefs)
                    {
                        ResearchUnlocks researchDefResearchUnlocks = researchDef.GetModExtension<ResearchUnlocks>();

                        if (researchDef != proj &&
                            !researchDef.IsFinished &&
                            researchDefResearchUnlocks != null &&
                            researchDefResearchUnlocks.researchUnlocks.Contains(unlock))
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

            if (proj.hiddenPrerequisites != null)
            {
                for (int i = 0; i < proj.hiddenPrerequisites.Count; i++)
                {
                    if (!proj.hiddenPrerequisites[i].IsFinished)
                    {
                        Find.ResearchManager.FinishProject(proj.hiddenPrerequisites[i]);
                    }
                }
            }
        }
    }
}