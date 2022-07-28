using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ThunderKit.Core.Paths;
using UnityEditor;
using UnityEngine;

namespace EditorHelpers
{
    public static class GenerateContentCode
    {
        private const string @class = "Content";
        private const string @namespace = "Archangel";
        private const string contentScriptPath = "<ContentScript>";

        [MenuItem("Archangel/Generate content script", isValidateFunction: true)]
        public static bool Validate()
        {
            return Selection.activeObject is SerializableContentPack;
        }

        [MenuItem("Archangel/Generate content script", priority = 10)]
        public static void ProcessContentProvider()
        {
            var stringBuilder = new StringBuilder();
            var contentPack = Selection.activeObject as SerializableContentPack;
            
            stringBuilder.Append(
$@"using RoR2.ContentManagement;
using System.Collections.Generic;
using RoR2;
using System;
using RoR2.Skills;
using EntityStates;

namespace {@namespace}
{{
    public partial class {@class}
    {{
        private static string ContentPackPath {{ get; }} = ""{AssetDatabase.GetAssetPath(contentPack)}"";

        private void PopulateAssetLoadDispatchers(List<Action> output, ContentPack contentPack)
        {{");

            stringBuilder.Append(contentPack.artifactDefs.Length == 0 ? "" : GenerateAction("Artifacts", "artifactDefs"));
            stringBuilder.Append(contentPack.itemDefs.Length == 0 ? "" : GenerateAction("Items", "itemDefs"));
            stringBuilder.Append(contentPack.equipmentDefs.Length == 0 ? "" : GenerateAction("Equipment", "equipmentDefs"));
            stringBuilder.Append(contentPack.buffDefs.Length == 0 ? "" : GenerateAction("Buffs", "buffDefs"));
            stringBuilder.Append(contentPack.eliteDefs.Length == 0 ? "" : GenerateAction("Elites", "eliteDefs"));
            stringBuilder.Append(contentPack.gameEndingDefs.Length == 0 ? "" : GenerateAction("GameEndings", "gameEndingDefs"));
            stringBuilder.Append(contentPack.survivorDefs.Length == 0 ? "" : GenerateAction("Survivors", "survivorDefs"));
            stringBuilder.Append(contentPack.skillDefs.Length == 0 ? "" : GenerateAction("Skills", "skillDefs"));

            stringBuilder.Append(@"
        }");

            GenerateFields(contentPack.artifactDefs.Select(el => ((ScriptableObject)el).name), "Artifacts", "ArtifactDef", stringBuilder);
            GenerateFields(contentPack.itemDefs.Select(el => ((ScriptableObject)el).name), "Items", "ItemDef", stringBuilder);
            GenerateFields(contentPack.equipmentDefs.Select(el => ((ScriptableObject)el).name), "Equipment", "EquipmentDef", stringBuilder);
            GenerateFields(contentPack.buffDefs.Select(el => ((ScriptableObject)el).name), "Buffs", "BuffDef", stringBuilder);
            GenerateFields(contentPack.eliteDefs.Select(el => ((ScriptableObject)el).name), "Elites", "EliteDef", stringBuilder);
            GenerateFields(contentPack.gameEndingDefs.Select(el => ((ScriptableObject)el).name), "GameEndings", "GameEndingDef", stringBuilder);
            GenerateFields(contentPack.survivorDefs.Select(el => ((ScriptableObject)el).name), "Survivors", "SurvivorDef", stringBuilder);
            GenerateFields(contentPack.skillDefs.Select(el => ((ScriptableObject)el).name), "Skills", "SkillDef", stringBuilder);

            stringBuilder.Append(@"
    }
}");

            var path = contentScriptPath.Resolve(null, null);
            File.WriteAllText(path, stringBuilder.ToString());
            AssetDatabase.ImportAsset(path);
        }

        private static string GenerateAction(string typeName, string fieldName)
        {
            return $@"
            output.Add(() => ContentLoadHelper.PopulateTypeFields(typeof({typeName}), contentPack.{fieldName}));";
        }

        private static void GenerateFields(IEnumerable<string> fields, string className, string fieldTypeName, StringBuilder stringBuilder)
        {
            if (fields.Count() == 0)
            {
                return;
            }

            stringBuilder.Append($@"
        public class {className}
        {{");

            foreach (var field in fields)
            {
                stringBuilder.Append($@"
            public static readonly {fieldTypeName} {field};");
            }

            stringBuilder.Append(@"
        }");
        }
    }
}