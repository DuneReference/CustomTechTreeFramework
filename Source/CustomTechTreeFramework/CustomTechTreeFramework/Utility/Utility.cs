using Verse;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DuneRef_CustomTechTreeFramework
{
    public static class Utility
    {
        // Until I find a reason not to, I've removed all the standard researches from being added.
        public static List<Def> GetUnlocksForResearch(ResearchProjectDef proj)
        {
            bool hasCarpet = false;
            bool hasFineCarpet = false;
            bool hasMindbendCarpet = false;
            bool hasHexCarpet = false;
            bool hasMorbidCarpet = false;

            return DefDatabase<RecipeDef>.AllDefs.Where(x =>
            {
                bool found = false;
                bool CEexit = false;

                if (Startup.usingCE && CustomTechTreeFrameworkSettings.removeCEAmmo && x.conceptLearned == CustomTechTreeFramework_DefOf.CE_AmmoCrafting)
                {
                    CEexit = true;
                }

                if (!CEexit)
                {
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

                        if (found)
                        {
                            if (x.defName.ToLower().Contains("carpet"))
                            {
                                bool foundCarpetType = false;

                                if (x.defName.ToLower().Contains("morbid"))
                                {
                                    if (!hasMorbidCarpet)
                                    {
                                        hasMorbidCarpet = true;
                                    } else
                                    {
                                        found = false;
                                    }

                                    foundCarpetType = true;
                                }

                                if (!foundCarpetType && x.defName.ToLower().Contains("transhumanist"))
                                {
                                    if (!hasHexCarpet)
                                    {
                                        hasHexCarpet = true;
                                    }
                                    else
                                    {
                                        found = false;
                                    }

                                    foundCarpetType = true;
                                }

                                if (!foundCarpetType && x.defName.ToLower().Contains("mindbend"))
                                {
                                    if (!hasMindbendCarpet)
                                    {
                                        hasMindbendCarpet = true;
                                    }
                                    else
                                    {
                                        found = false;
                                    }

                                    foundCarpetType = true;
                                }

                                if (!foundCarpetType && x.defName.ToLower().Contains("fine"))
                                {
                                    if (!hasFineCarpet)
                                    {
                                        hasFineCarpet = true;
                                    }
                                    else
                                    {
                                        found = false;
                                    }

                                    foundCarpetType = true;
                                }

                                if (!foundCarpetType)
                                {
                                    if (!hasCarpet)
                                    {
                                        hasCarpet = true;
                                    }
                                    else
                                    {
                                        found = false;
                                    }

                                    foundCarpetType = true;
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