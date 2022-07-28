using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EditorHelpers
{
    public static class FixBlenderToFbxConversion
    {
        private const string scriptName = "Unity-BlenderToFBX.py";
        private static readonly string scriptRelativePath = Path.Combine("Data", "Tools", scriptName);
        private static readonly string checkFilePath = Path.Combine("Temp", ".fixblendertofbxconversion");

        private static readonly Regex bakeAllActionsRegex = new Regex(@"(?'line'bake_anim_use_all_actions=)(?'value'(True|False))", RegexOptions.Compiled);

        [InitializeOnLoadMethod]
        public static void Execute()
        {
            if (File.Exists(checkFilePath))
            {
                return;
            }

            File.WriteAllText(checkFilePath, "");

            var appDirectory = Path.GetDirectoryName(EditorApplication.applicationPath);
            var scriptPath = Path.Combine(appDirectory, scriptRelativePath);

            if (!File.Exists(scriptPath))
            {
                EditorUtility.DisplayDialog($"Could not find importer script", $"Could not find \"{scriptName}\" script.", "OK");
                return;
            }

            var script = File.ReadAllText(scriptPath);
            var match = bakeAllActionsRegex.Match(script);
            if (!match.Success || match.Groups["value"].Value == "True")
            {
                return;
            }

            if (!EditorUtility.DisplayDialog("Blender to FBX conversion", $"Unity broke animations import for .blend files in this project by fixing something else.\nFix issue by modifying \"{scriptName}\" file (project won't work correctly otherwise)?\nThis may impact your other projects as it modifies your Unity installation.", "Yes", "No"))
            {
                return;
            }

            try
            {
                File.WriteAllText(scriptPath, bakeAllActionsRegex.Replace(script, (m) => m.Groups["line"].Value + "True"));
            }
            catch (UnauthorizedAccessException)
            {
                EditorUtility.DisplayDialog("Error", $"Couldn't fix script because access to \"{scriptPath}\" is denied. Restart Unity as admin or change \"bake_anim_use_all_actions\" to \"True\" yourself", "OK");
            }
        }
    }
}
