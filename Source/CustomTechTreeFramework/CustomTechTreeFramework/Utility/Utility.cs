using Verse;
using System.Collections.Generic;
using System.Linq;

namespace DuneRef_CustomTechTreeFramework
{
    public static class Utility
    {
        // Until I find a reason not to, I've removed all the standard researches from being added.
        public static List<Def> GetUnlocksForResearch(ResearchProjectDef proj)
        {
            return DefDatabase<RecipeDef>.AllDefs.Where(x =>
            {
                bool found = false;

                if (proj.GetModExtension<ResearchUnlocks>() != null)
                {
                    found = x.researchPrerequisite == proj ||
                           (x.researchPrerequisites != null &&
                            x.researchPrerequisites.Contains(proj));

                    if (!found)
                    {
                        foreach (ResearchProjectDef unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                        {
                            found = x.researchPrerequisite == unlock ||
                                   (x.researchPrerequisites != null &&
                                    x.researchPrerequisites.Contains(unlock));
                            if (found) break;
                        }
                    }
                }

                return found;
            })
            .SelectMany(x => x.products.Select(y => (Def)y.thingDef))
            .OrderBy(x => x.label)
            .Concat(
                DefDatabase<ThingDef>.AllDefs
                    .Where(x =>
                    {
                        bool found = false;

                        if (proj.GetModExtension<ResearchUnlocks>() != null)
                        {
                            found = x.researchPrerequisites != null &&
                                    x.researchPrerequisites.Contains(proj);

                            if (!found)
                            {
                                foreach (ResearchProjectDef unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                                {
                                    found = x.researchPrerequisites != null &&
                                            x.researchPrerequisites.Contains(unlock);
                                    if (found) break;
                                }
                            }
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Concat(
                DefDatabase<ThingDef>.AllDefs
                    .Where(x =>
                    {
                        bool found = false;

                        if (proj.GetModExtension<ResearchUnlocks>() != null)
                        {
                            found = x.plant != null &&
                                    x.plant.sowResearchPrerequisites != null &&
                                    x.plant.sowResearchPrerequisites.Contains(proj);

                            if (!found)
                            {
                                foreach (ResearchProjectDef unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                                {
                                    found = x.plant != null &&
                                            x.plant.sowResearchPrerequisites != null &&
                                            x.plant.sowResearchPrerequisites.Contains(unlock);
                                    if (found) break;
                                }
                            }
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Concat(
                DefDatabase<TerrainDef>.AllDefs
                    .Where(x =>
                    {
                        bool found = false;

                        if (proj.GetModExtension<ResearchUnlocks>() != null)
                        {
                            found = x.researchPrerequisites != null &&
                                    x.researchPrerequisites.Contains(proj);

                            if (!found)
                            {
                                foreach (ResearchProjectDef unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                                {
                                    found = x.researchPrerequisites != null &&
                                            x.researchPrerequisites.Contains(unlock);
                                    if (found) break;
                                }
                            }
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Distinct()
            .ToList();
        }
    }
}