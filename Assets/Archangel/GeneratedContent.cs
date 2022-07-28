using RoR2.ContentManagement;
using System.Collections.Generic;
using RoR2;
using System;
using RoR2.Skills;
using EntityStates;

namespace Archangel
{
    public partial class Content
    {
        private static string ContentPackPath { get; } = "Assets/Resources/ContentPack.asset";

        private void PopulateAssetLoadDispatchers(List<Action> output, ContentPack contentPack)
        {
            output.Add(() => ContentLoadHelper.PopulateTypeFields(typeof(Survivors), contentPack.survivorDefs));
            output.Add(() => ContentLoadHelper.PopulateTypeFields(typeof(Skills), contentPack.skillDefs));
        }
        public class Survivors
        {
            public static readonly SurvivorDef Archangel;
        }
        public class Skills
        {
            public static readonly SkillDef ArchangelPrimaryAttack;
            public static readonly SkillDef ArchangelSecondaryAttack;
            public static readonly SkillDef ArchangelSpecial;
        }
    }
}