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

        [DebugAction("Research", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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