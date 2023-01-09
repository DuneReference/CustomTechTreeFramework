using System;

using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using ResearchPal;
using DubsMintMenus;

namespace DuneRef_CustomTechTreeFramework
{
    public static class ResearchPalForkedPatches
    {
        public static readonly Type patchType = typeof(ResearchPalForkedPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Finish linked projects
            Harm.Patch(AccessTools.Method(typeof(ResearchPal.HarmonyPatches_Queue.DoCompletionDialog), "Postfix"), prefix: new HarmonyMethod(CommonPatches.patchType, nameof(CommonPatches.FinishProjectOmniFix)));

            // Show icons of linked projects for my projects 
            Harm.Patch(AccessTools.Method(typeof(ResearchPal.ResearchProjectDef_Extensions), nameof(ResearchPal.ResearchProjectDef_Extensions.GetUnlockDefs)), prefix: new HarmonyMethod(patchType, nameof(GetUnlockDefsPrefix)));

            // Hide projects that I designate for hiding.
            MethodInfo populateNodesInnerMethod = AccessTools.FindIncludingInnerTypes(
                typeof(ResearchPal.Tree),
                (type) => AccessTools.FirstMethod(
                    type,
                    (method) => method.Name.Contains("<PopulateNodes>") && method.ReturnType == typeof(bool)
                )
            );
            Harm.Patch(populateNodesInnerMethod, postfix: new HarmonyMethod(patchType, nameof(ResearchPalTreePopulateNodesHiddenPostfix)));
        }

        public static bool GetUnlockDefsPrefix(ref List<Def> __result, ResearchProjectDef research, ref Dictionary<Def, List<Def>> ____unlocksCache)
        {
            if (____unlocksCache.ContainsKey(research))
            {
                __result = ____unlocksCache[research];
            }
            else
            {
                __result = Utility.GetUnlocksForResearch(research)
                    .Where(x => x.IconTexture() != null)
                    .ToList();

                ____unlocksCache.Add(research, __result);
            }

            return false;
        }

        public static bool ResearchPalTreePopulateNodesHiddenPostfix(bool __result, ResearchProjectDef p)
        {
            return __result || (p.tab != null && p.tab == RimCivTechTree_DefOf.DuneRef_Hidden);
        }
    }
}