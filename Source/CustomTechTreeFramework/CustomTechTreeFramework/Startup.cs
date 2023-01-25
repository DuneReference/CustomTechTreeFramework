using Verse;
using UnityEngine;

namespace DuneRef_CustomTechTreeFramework
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        public static bool usingCE = ModLister.HasActiveModWithName("Combat Extended");
        static Startup()
        {
        }
    }

    public class CustomTechTreeFrameworkSettings : ModSettings
    {
        public static bool removeCostFactor = true;
        public static bool removeCEAmmo = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref removeCostFactor, "removeCostFactor", removeCostFactor);
            Scribe_Values.Look(ref removeCEAmmo, "removeCEAmmo", removeCEAmmo);
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
            listingStandard.CheckboxLabeled("CE Ammo does not show up in tech tree", ref CustomTechTreeFrameworkSettings.removeCEAmmo, "CE Ammo does not show up in tech tree");
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