using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace DuneRef_CustomTechTreeFramework
{
    public static class SemiRandomResearchPatches
    {
        public static readonly Type PatchType = typeof(SemiRandomResearchPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Hide projects that I designate for hiding.
            Harm.Patch(AccessTools.Method(typeof(CM_Semi_Random_Research.ResearchTracker), "GetResearchableProjects"), transpiler: new HarmonyMethod(PatchType, nameof(GetResearchableProjectsTranspiler)));
        }
        
        public static IEnumerable<CodeInstruction> GetResearchableProjectsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                var hideResearchDefsInstructions = new CodeMatch[]{
                    // IL_0080: call class [mscorlib]System.Collections.Generic.List`1<!0> class ['Assembly-CSharp']Verse.DefDatabase`1<class ['Assembly-CSharp']Verse.ResearchProjectDef>::get_AllDefsListForReading()
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_0085: ldarg.0     // V_0
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    // IL_0086: ldftn instance bool CM_Semi_Random_Research.ResearchTracker::'<GetResearchableProjects>b__12_2'(class ['Assembly-CSharp']Verse.ResearchProjectDef)
                    new CodeMatch(i => i.opcode == OpCodes.Ldftn),
                    // IL_008c: newobj instance void class [mscorlib]System.Func`2<class ['Assembly-CSharp']Verse.ResearchProjectDef, bool>::.ctor(object, native int)
                    new CodeMatch(i => i.opcode == OpCodes.Newobj),
                    // IL_0091: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ['Assembly-CSharp']Verse.ResearchProjectDef>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_0096: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class ['Assembly-CSharp']Verse.ResearchProjectDef>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_009b: stloc.2      // researchProjectDefList
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_2)
                };
        
                return new CodeMatcher(instructions, generator)
                    .Start()
                    .MatchStartForward(hideResearchDefsInstructions)
                    .Advance(1)
                    .ThrowIfInvalid("Couldn't find the hideResearchDefsInstructions instructions")
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(PatchType, nameof(SemiRandomResearchPatches.CullHiddenResearch))))
                    .InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error($"[DuneRef_CustomTechTreeFramework] : {ex}");
                return instructions;
            }
        }
        
        public static List<ResearchProjectDef> CullHiddenResearch(List<ResearchProjectDef> source)
        {
            return source.Where((x) => x.tab != CustomTechTreeFramework_DefOf.DuneRef_Hidden).ToList();
        }
    }
}