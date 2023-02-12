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
            var hasCarpet = false;
            var hasFineCarpet = false;
            var hasMindbendCarpet = false;
            var hasHexCarpet = false;
            var hasMorbidCarpet = false;

            return DefDatabase<RecipeDef>.AllDefs.Where(x =>
            {
                var found = false;
                var CEexit = Startup.UsingCE &&
                             CustomTechTreeFrameworkSettings.RemoveCEAmmo &&
                             x.conceptLearned == CustomTechTreeFramework_DefOf.CE_AmmoCrafting;

                if (CEexit || proj.GetModExtension<ResearchUnlocks>() == null)
                {
                    return false;
                }

                found = x.researchPrerequisite == proj ||
                        (x.researchPrerequisites != null &&
                         x.researchPrerequisites.Contains(proj));

                if (found)
                {
                    return true;
                }

                foreach (var unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                {
                    found = x.researchPrerequisite == unlock ||
                            (x.researchPrerequisites != null &&
                             x.researchPrerequisites.Contains(unlock));
                    if (found) break;
                }

                return found;
            })
            .SelectMany(x => x.products.Select(y => (Def)y.thingDef))
            .OrderBy(x => x.label)
            .Concat(
                DefDatabase<ThingDef>.AllDefs
                    .Where(x =>
                    {
                        if (proj.GetModExtension<ResearchUnlocks>() == null)
                        {
                            return false;
                        }

                        var found = x.researchPrerequisites != null &&
                                   x.researchPrerequisites.Contains(proj);

                        if (found)
                        {
                            return true;
                        }

                        foreach (var unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                        {
                            found = x.researchPrerequisites != null &&
                                    x.researchPrerequisites.Contains(unlock);
                            if (found) break;
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Concat(
                DefDatabase<ThingDef>.AllDefs
                    .Where(x =>
                    {
                        var found = false;

                        if (proj.GetModExtension<ResearchUnlocks>() == null)
                        {
                            return false;
                        }

                        found = x.plant != null &&
                                x.plant.sowResearchPrerequisites != null &&
                                x.plant.sowResearchPrerequisites.Contains(proj);

                        if (found)
                        {
                            return true;
                        }

                        foreach (var unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                        {
                            found = x.plant != null &&
                                    x.plant.sowResearchPrerequisites != null &&
                                    x.plant.sowResearchPrerequisites.Contains(unlock);
                            if (found) break;
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Concat(
                DefDatabase<TerrainDef>.AllDefs
                    .Where(x =>
                    {
                        var found = false;

                        if (proj.GetModExtension<ResearchUnlocks>() != null)
                        {
                            found = x.researchPrerequisites != null &&
                                    x.researchPrerequisites.Contains(proj);

                            if (!found)
                            {
                                foreach (var unlock in proj.GetModExtension<ResearchUnlocks>().researchUnlocks)
                                {
                                    found = x.researchPrerequisites != null &&
                                            x.researchPrerequisites.Contains(unlock);
                                    if (found) break;
                                }
                            }
                        }

                        if (!found)
                        {
                            return false;
                        }

                        if (!x.defName.ToLower().Contains("carpet"))
                        {
                            return true;
                        }

                        var foundCarpetType = false;

                        if (x.defName.ToLower().Contains("morbid"))
                        {
                            if (!hasMorbidCarpet)
                            {
                                hasMorbidCarpet = true;
                            }
                            else
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

                        if (foundCarpetType)
                        {
                            return found;
                        }

                        if (!hasCarpet)
                        {
                            hasCarpet = true;
                        }
                        else
                        {
                            found = false;
                        }

                        return found;
                    })
                    .OrderBy(x => x.label))
            .Distinct()
            .ToList();
        }
    }
}