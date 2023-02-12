using System;

using Verse;
using HarmonyLib;
using System.Linq;

namespace DuneRef_CustomTechTreeFramework
{
    public static class CommonPatches
    {
        public static readonly Type PatchType = typeof(CommonPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void FinishProjectOmniFix(ResearchProjectDef proj)
        {
            if (!proj.IsFinished)
                return;

            var projResearchUnlocks = proj.GetModExtension<ResearchUnlocks>();

            if (projResearchUnlocks != null)
            {
                foreach (var unlock in projResearchUnlocks.researchUnlocks)
                {
                    var researchDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                                                            .Where(x => x.tab != CustomTechTreeFramework_DefOf.DuneRef_Hidden)
                                                            .ToList();

                    var notFinished = false;

                    foreach (var researchDef in researchDefs)
                    {
                        var researchDefResearchUnlocks = researchDef.GetModExtension<ResearchUnlocks>();

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

            if (proj.hiddenPrerequisites == null)
                return;

            foreach(var hiddenPrerequisite in proj.hiddenPrerequisites)
            {
                if (!hiddenPrerequisite.IsFinished)
                {
                    Find.ResearchManager.FinishProject(hiddenPrerequisite);
                }
            }
        }
    }
}