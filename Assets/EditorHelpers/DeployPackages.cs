using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThunderKit.Core.Attributes;
using ThunderKit.Core.Paths;
using UnityEditor;
using UnityEngine;

namespace ThunderKit.Core.Pipelines.Jobs
{
    [PipelineSupport(typeof(Pipeline))]
    public class DeployPackages : FlowPipelineJob
    {
        protected override Task ExecuteInternal(Pipeline pipeline)
        {
            var success = false;

            string stagingRoot;
            try
            {
                stagingRoot = "<ManifestStagingRoot>".Resolve(pipeline, this);
            }
            catch
            {
                return Task.CompletedTask;
            }

            var outputPath = "<BepInEx_Config_Folder>".Resolve(pipeline, this);
            if (CopyFiles(Path.Combine(stagingRoot, "config"), outputPath) ||
                CopyFiles(Path.Combine(stagingRoot, "BepInEx", "config"), outputPath))
            {
                success = true;
            }

            outputPath = "<BepInEx_Monomod>".Resolve(pipeline, this);
            if (CopyFiles("<ManifestStagingRoot>/monomod", outputPath) ||
                CopyFiles("<ManifestStagingRoot>/BepInEx/monomod", outputPath))
            {
                success = true;
            }

            outputPath = "<BepInEx_Patchers>".Resolve(pipeline, this);
            if (CopyFiles(Path.Combine(stagingRoot, "patchers"), outputPath) ||
                CopyFiles(Path.Combine(stagingRoot, "BepInEx", "patchers"), outputPath))
            {
                success = true;
            }

            outputPath = "<BepInEx_Plugins>".Resolve(pipeline, this);
            if (CopyFiles(Path.Combine(stagingRoot, "plugins"), outputPath) ||
                CopyFiles(Path.Combine(stagingRoot, "BepInEx", "plugins"), outputPath))
            {
                success = true;
            }

            if (!success)
            {
                CopyFiles(stagingRoot, outputPath);
            }

            return Task.CompletedTask;
        }

        private bool CopyFiles(string source, string destination)
        {
            if (!Directory.Exists(source))
            {
                return false;
            }

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                var destinationFile = Path.Combine(destination, filePath.Substring(source.Length + 1));
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                FileUtil.ReplaceFile(filePath, destinationFile);
            }

            return files.Length > 0;
        }
    }
}
