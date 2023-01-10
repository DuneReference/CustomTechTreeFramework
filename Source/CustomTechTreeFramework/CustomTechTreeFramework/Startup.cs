using Verse;
using UnityEngine;

namespace DuneRef_CustomTechTreeFramework
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
        }
    }

    public class CustomTechTreeFrameworkSettings : ModSettings
    {
        public static bool removeCostFactor;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref removeCostFactor, "removeCostFactor", true);
            base.ExposeData();
        }
    }

    public class CustomTechTreeFrameworkMod : Mod
    {
        public CustomTechTreeFrameworkSettings settings;

        public CustomTechTreeFrameworkMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<CustomTechTreeFrameworkSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Research Cost is not modified by TechLevel", ref CustomTechTreeFrameworkSettings.removeCostFactor, "Research Cost is not modified by TechLevel");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "CustomTechTreeFramework";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();

            VanillaPatches.UpdateCostFactorPatch();
        }
    }
}