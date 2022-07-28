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
            if (CopyFiles(pipeline, "<ManifestStagingRoot>/plugins", "<BepInEx_Plugins>") ||
                CopyFiles(pipeline, "<ManifestStagingRoot>/BepInEx/plugins", "<BepInEx_Plugins>"))
            {
                success = true;
            }

            if (CopyFiles(pipeline, "<ManifestStagingRoot>/monomod", "<BepInEx_Monomod>") ||
                CopyFiles(pipeline, "<ManifestStagingRoot>/BepInEx/monomod", "<BepInEx_Monomod>"))
            {
                success = true;
            }

            if (CopyFiles(pipeline, "<ManifestStagingRoot>/BepInEx/patchers", "<BepInEx_Patchers>") ||
                CopyFiles(pipeline, "<ManifestStagingRoot>/patchers", "<BepInEx_Patchers>"))
            {
                success = true;
            }

            if (!success)
            {
                CopyFiles(pipeline, "<ManifestStagingRoot>", "<BepInEx_Plugins>");
            }

            return Task.CompletedTask;
        }

        private bool CopyFiles(Pipeline pipeline, string sourcePath, string destinationPath)
        {
            string source;
            try
            {
                source = sourcePath.Resolve(pipeline, this);
            }
            catch
            {
                return false;
            }

            if (!Directory.Exists(source))
            {
                return false;
            }

            var destination = destinationPath.Resolve(pipeline, this);
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
