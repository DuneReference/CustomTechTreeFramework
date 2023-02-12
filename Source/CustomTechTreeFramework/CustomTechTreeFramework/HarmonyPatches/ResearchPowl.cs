using System;

using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using ResearchPowl;

namespace DuneRef_CustomTechTreeFramework
{
    public static class ResearchPowlPatches
    {
        public static readonly Type PatchType = typeof(ResearchPowlPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Finish linked projects
            Harm.Patch(AccessTools.Method(typeof(ResearchPowl.DoCompletionDialog), "Postfix"), prefix: new HarmonyMethod(CommonPatches.PatchType, nameof(CommonPatches.FinishProjectOmniFix)));

            // Show icons of linked projects for my projects 
            var GetUnlockDefsInnerMethod = AccessTools.FindIncludingInnerTypes(
                typeof(ResearchPowl.ResearchNode),
                (type) => AccessTools.FirstMethod(
                    type,
                    (method) => method.Name.Contains("<Unlocks>") &&
                                method.ReturnType == typeof(List<Def>) &&
                                method.GetParameters().Length > 0 &&
                                method.GetParameters()[0].ParameterType == typeof(ResearchProjectDef)
                )
            );
            Harm.Patch(GetUnlockDefsInnerMethod, prefix: new HarmonyMethod(PatchType, nameof(GetUnlockDefsPrefix)));

            // Hide projects that I designate for hiding.
            var populateNodesInnerMethod = AccessTools.FindIncludingInnerTypes(
                typeof(ResearchPowl.Tree),
                (type) => AccessTools.FirstMethod(
                    type,
                    (method) => method.Name.Contains("<InitializeNodesStructures>") && 
                                method.ReturnType == typeof(bool) && 
                                method.GetParameters().Length > 0 && 
                                method.GetParameters()[0].ParameterType == typeof(ResearchProjectDef)
                )
            );
            Harm.Patch(populateNodesInnerMethod, postfix: new HarmonyMethod(PatchType, nameof(ResearchPowlTreePopulateNodesHiddenPostfix)));
        }
        
        public static bool GetUnlockDefsPrefix(ref List<Def> __result, ResearchProjectDef research, ref Dictionary<Def, List<Def>> ____unlocksCache)
        {
            if (____unlocksCache.ContainsKey(research))
            {
                __result = ____unlocksCache[research];
            }
            else
            {
                // May want to refactor this like Owlchemist did to remove LINQ and unneccessary passes through the list.
                // Keeping it like this for now because I'm lazy.
                __result = Utility.GetUnlocksForResearch(research)
                    .Where(x => x.IconTexture() != null)
                    .ToList();
        
                ____unlocksCache.Add(research, __result);
            }
        
            return false;
        }
        
        public static bool ResearchPowlTreePopulateNodesHiddenPostfix(bool __result, ResearchProjectDef p)
        {
            return __result || (p.tab != null && p.tab == CustomTechTreeFramework_DefOf.DuneRef_Hidden);
        }
    }
}