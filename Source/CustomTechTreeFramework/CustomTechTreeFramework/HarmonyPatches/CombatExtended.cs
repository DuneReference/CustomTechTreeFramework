using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace DuneRef_CustomTechTreeFramework
{
    public static class CombatExtendedPatches
    {
        public static readonly Type PatchType = typeof(CombatExtendedPatches);
        public static Harmony Harm = HarmonyPatches.Harm;

        public static void Patches()
        {
            // Do recipe Inheritance for workbenches after CE ammo additions.
            Harm.Patch(AccessTools.Method(typeof(CombatExtended.AmmoInjector), "Inject"), postfix: new HarmonyMethod(PatchType, nameof(AddRecipesPostfix)));
        }

        public static void AddRecipesPostfix()
        {
            var defs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < defs.Count; i++)
            {
                var self = defs[i];
                if (self.IsWorkTable && self.GetModExtension<RecipeInheritance.ThingDefExtension>() is RecipeInheritance.ThingDefExtension ext && ext.inheritRecipesFrom != null)
                {
                    var list = new List<RecipeDef>(self.AllRecipes);
                    RecipeInheritance.NonPublicFields.ThingDef_allRecipesCached.SetValue(self, null);

                    // Log.Message($"[RecipeInheritance] for worktable: {self.defName}");

                    for (int j = 0; j < ext.inheritRecipesFrom.Count; j++)
                    {
                        var worktable = ext.inheritRecipesFrom[j];
                        // Log.Message($"[RecipeInheritance] inheriting recipes from  {worktable.defName}");
                        var recipeDefs = worktable.AllRecipes ?? new List<RecipeDef>();

                        for (int k = 0; k < recipeDefs.Count; k++)
                        {
                            var recipeDef = worktable.AllRecipes[k];
                            if (ext.Allows(recipeDef))
                            {
                                if (self.recipes == null)
                                {
                                    self.recipes = new List<RecipeDef>();
                                }

                                // Log.Message($"[RecipeInheritance] Attempting to add {recipeDef.defName}");

                                if (!list.Contains(recipeDef))
                                {
                                    // Log.Message($"[RecipeInheritance] Added {recipeDef.defName}");
                                    self.recipes.Add(recipeDef);
                                }
                            }
                        }
                    }

                    // Log.Message("---");
                }
            }
        }
    }
}