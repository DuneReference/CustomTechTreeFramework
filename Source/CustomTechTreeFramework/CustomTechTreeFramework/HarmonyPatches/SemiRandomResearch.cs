using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DubsMintMenus;
using HarmonyLib;
using Verse;

namespace DuneRef_CustomTechTreeFramework
{
    public static class SemiRandomResearchPatches
    {
        public static readonly Type patchType = typeof(SemiRandomResearchPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Hide projects that I designate for hiding.
            Harm.Patch(AccessTools.Method(typeof(CM_Semi_Random_Research.ResearchTracker), "GetResearchableProject"), transpiler: new HarmonyMethod(patchType, nameof(GetResearchableProjectTranspiler)));
        }

        public static IEnumerable<CodeInstruction> GetResearchableProjectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatch[] hideResearchDefsInstructions = new CodeMatch[]{
                    // IL_00ef: call         class [mscorlib] System.Collections.Generic.List`1<!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/> class ['Assembly-CSharp'] Verse.DefDatabase`1<class ['Assembly-CSharp'] Verse.ResearchProjectDef>::get_AllDefsListForReading()
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_00f4: ldloc.0      // V_0
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_0),
                    // IL_00f5: ldftn instance bool CM_Semi_Random_Research.ResearchTracker/'<>c__DisplayClass12_0'::'<GetResearchableProject>b__2'(class ['Assembly-CSharp'] Verse.ResearchProjectDef)
                    new CodeMatch(i => i.opcode == OpCodes.Ldftn),
                    // IL_00fb: newobj instance void class [mscorlib] System.Func`2<class ['Assembly-CSharp'] Verse.ResearchProjectDef, bool>::.ctor(object, native int)
                    new CodeMatch(i => i.opcode == OpCodes.Newobj),
                    // IL_0100: call class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/> [System.Core] System.Linq.Enumerable::Where<class ['Assembly-CSharp'] Verse.ResearchProjectDef>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/>, class [mscorlib] System.Func`2<!!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/, bool>)
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_0105: call class [mscorlib] System.Collections.Generic.List`1<!!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/> [System.Core] System.Linq.Enumerable::ToList<class ['Assembly-CSharp'] Verse.ResearchProjectDef>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0/*class ['Assembly-CSharp']Verse.ResearchProjectDef*/>)
                    new CodeMatch(i => i.opcode == OpCodes.Call),
                    // IL_010a: stloc.3      // researchProjectDefList
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_3)
                };

                return new CodeMatcher(instructions, generator)
                    .Start()
                    .MatchStartForward(hideResearchDefsInstructions)
                    .Advance(1)
                    .ThrowIfInvalid("Couldn't find the hideResearchDefsInstructions instructions")
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(SemiRandomResearchPatches.CullHiddenResearch))))
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
            return source.Where((x) => x.tab != RimCivTechTree_DefOf.DuneRef_Hidden).ToList();
        }
    }
}