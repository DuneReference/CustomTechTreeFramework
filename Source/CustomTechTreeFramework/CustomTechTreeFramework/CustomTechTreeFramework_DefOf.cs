using RimWorld;

namespace DuneRef_CustomTechTreeFramework
{
    public class MayRequireCEAttribute : MayRequireAttribute
    {
        public MayRequireCEAttribute()
            : base("ceteam.combatextended")
        {
        }
    }

    [DefOf]
    public class CustomTechTreeFramework_DefOf
    {
        public static ResearchTabDef DuneRef_Hidden;
        [MayRequireCE] public static ConceptDef CE_AmmoCrafting;
    }
}