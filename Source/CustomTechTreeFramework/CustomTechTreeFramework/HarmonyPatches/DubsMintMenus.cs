using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DubsMintMenus;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DuneRef_CustomTechTreeFramework
{
    public static class DubsMintMenusPatches
    {
        public static readonly Type patchType = typeof(DubsMintMenusPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Finish linked projects
            Harm.Patch(AccessTools.Method(typeof(DubsMintMenus.HarmonyPatches.Patch_FinishProject), "Postfix"), prefix: new HarmonyMethod(CommonPatches.patchType, nameof(CommonPatches.FinishProjectOmniFix)));

            // Show icons of linked projects for my projects 
            Harm.Patch(AccessTools.Method(typeof(DubsMintMenus.MainTabWindow_MintResearch), nameof(DubsMintMenus.MainTabWindow_MintResearch.AllUnlockedBy)), prefix: new HarmonyMethod(patchType, nameof(AllUnlockedByPrefix)));

            // Hide projects that I designate for hiding.
            Harm.Patch(AccessTools.Method(typeof(DubsMintMenus.MainTabWindow_MintResearch), nameof(DubsMintMenus.MainTabWindow_MintResearch.AllProjectsForTab)), prefix: new HarmonyMethod(patchType, nameof(AllProjectsForTabPrefix)));
            Harm.Patch(AccessTools.Method(typeof(DubsMintMenus.MainTabWindow_MintResearch), "DrawModsList"), transpiler: new HarmonyMethod(patchType, nameof(DrawModsListTranspiler)));
            Harm.Patch(AccessTools.PropertyGetter(typeof(DubsMintMenus.MainTabWindow_MintResearch), "ProjectsAvailable"), prefix: new HarmonyMethod(patchType, nameof(ProjectsAvailablePrefix)));
        }

        public static bool AllUnlockedByPrefix(ref List<Def> __result, ResearchProjectDef proj, ref Dictionary<ResearchProjectDef, List<Def>> ___CachedUnlocks)
        {
            if (!___CachedUnlocks.TryGetValue(proj, out _))
            {
                List<Def> unlocks = Utility.GetUnlocksForResearch(proj);

                ___CachedUnlocks.Add(proj, unlocks);
            }

            __result = ___CachedUnlocks[proj];
            return false;
        }

        public static bool AllProjectsForTabPrefix(ref List<ResearchProjectDef> __result, ResearchTabDef tab)
        {
            if (tab == RimCivTechTree_DefOf.DuneRef_Hidden)
            {
                __result = new List<ResearchProjectDef>();
                return false;
            }
            else
            {
                return true;
            }
        }

        public static IEnumerable<CodeInstruction> DrawModsListTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatch[] tabDefsListGeneration = new CodeMatch[]{
                    // IL_005b: starg.s      'box'
                    new CodeMatch(i => i.opcode == OpCodes.Starg_S),
                    // IL_005d: call         class [mscorlib]System.Collections.Generic.List`1<!0/*class ['Assembly-CSharp']RimWorld.ResearchTabDef*/> class ['Assembly-CSharp']Verse.DefDatabase`1<class ['Assembly-CSharp']RimWorld.ResearchTabDef>::get_AllDefsListForReading()
                    new CodeMatch(i => i.Calls(AccessTools.PropertyGetter(typeof(DefDatabase<ResearchTabDef>), nameof(DefDatabase<ResearchTabDef>.AllDefsListForReading)))),
                    // IL_0062: stloc.0      // source
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_0),
                };

                CodeMatch[] AllTabRemovalLines = new CodeMatch[]{
                    // [334 7 - 334 81]
                    // IL_00f0: ldarg.0      // this
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    // IL_00f1: ldc.r4       0.0
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4),
                    // IL_00f6: ldloc.2      // y1
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_2),
                    // IL_00f7: ldarga.s     'box'
                    new CodeMatch(i => i.opcode == OpCodes.Ldarga_S),
                    // IL_00f9: call instance float32[UnityEngine.CoreModule]UnityEngine.Rect::get_width()
                    new CodeMatch(i => i.Calls(AccessTools.PropertyGetter(typeof(UnityEngine.Rect), nameof(UnityEngine.Rect.width)))),
                    // IL_00fe: ldc.r4       30
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4),
                    // IL_0103: newobj instance void [UnityEngine.CoreModule]UnityEngine.Rect::.ctor(float32, float32, float32, float32)
                    new CodeMatch(i => i.opcode == OpCodes.Newobj),
                    // IL_0108: ldnull
                    new CodeMatch(i => i.opcode == OpCodes.Ldnull),
                    // IL_0109: call instance void DubsMintMenus.MainTabWindow_MintResearch::DrawModRow(valuetype[UnityEngine.CoreModule]UnityEngine.Rect, class ['Assembly-CSharp'] RimWorld.ResearchTabDef)
                    new CodeMatch(i => i.Calls(AccessTools.Method(typeof(DubsMintMenus.MainTabWindow_MintResearch), "DrawModRow", new Type[] { typeof(UnityEngine.Rect), typeof(ResearchTabDef) }))),
                    // 
                    // // [335 7 - 335 26]
                    // IL_010e: ldloc.2      // y1
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_2),
                    // IL_010f: ldc.r4       30
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4),
                    // IL_0114: add
                    new CodeMatch(i => i.opcode == OpCodes.Add),
                    // IL_0115: stloc.2      // y2
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_2)
                };

                return new CodeMatcher(instructions, generator)
                    .Start()
                    .MatchStartForward(tabDefsListGeneration)
                    .ThrowIfInvalid("Couldn't find the tabDefsListGeneration instructions")
                    .Advance(2)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(DubsMintMenusPatches.CullHiddenTab))))
                    .MatchStartForward(AllTabRemovalLines)
                    .ThrowIfInvalid("Couldn't find the AllTabRemovalLines instructions")
                    .RemoveInstructions(AllTabRemovalLines.Count())
                    .InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error($"[DuneReference] : {ex}");
                return instructions;
            }
        }

        public static List<ResearchTabDef> CullHiddenTab(List<ResearchTabDef> source)
        {
            return source.Where((x) => x.GetModExtension<HiddenTab>() == null || x.GetModExtension<HiddenTab>().hidden == false).ToList();
        }

        public static bool ProjectsAvailablePrefix(ref List<ResearchProjectDef> __result, object __instance)
        {
            MethodInfo Parc = AccessTools.Method(__instance.GetType(), "parc");

            __result = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                .Where(x => x.tab != RimCivTechTree_DefOf.DuneRef_Hidden)
                .Where(x => (bool)Parc.Invoke(__instance, new Object[] { x })).ToList();
            return false;
        }
    }
}