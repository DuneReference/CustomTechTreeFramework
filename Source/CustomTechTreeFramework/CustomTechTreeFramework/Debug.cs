using RimWorld;
using Verse;

namespace DuneRef_CustomTechTreeFramework
{
    [StaticConstructorOnStartup]
    public static class Debug
    {
        static Debug()
        {
        }

        [DebugAction("CustomTechTreeFramework", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void FinishCurrentResearch()
        {
            if (Find.ResearchManager.currentProj != null)
            {
                Find.ResearchManager.FinishProject(Find.ResearchManager.currentProj);
            }

            Messages.Message("Current research finished.", MessageTypeDefOf.TaskCompletion, false);
        }
    }
}