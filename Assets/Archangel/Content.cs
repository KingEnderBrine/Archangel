using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Archangel
{
    public partial class Content : IContentPackProvider
    {
        private const string bundleName = "Paladin_Alliance_Archangel";
        
        private static AssetBundle assetBundle;
        private static ContentPack contentPack;

        private static Content _instance;
        private static Content Instance { get => _instance ?? (_instance = new Content()); }

        public string identifier => ArchangelPlugin.PluginGUID;

        private Content() { }

        internal static void CollectProvider(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(Instance);
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            var assetBundleRequest = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ArchangelPlugin.Instance.Info.Location), bundleName));
            yield return HandleAsyncOperationProgress(args, assetBundleRequest, 0, 0.25f);
            assetBundle = assetBundleRequest.assetBundle;

            var serializableContentRequest = assetBundle.LoadAssetAsync<SerializableContentPack>(ContentPackPath);
            yield return HandleAsyncOperationProgress(args, serializableContentRequest, 0.25f, 0.9f);

            var serializableContent = serializableContentRequest.asset as SerializableContentPack;
            contentPack = serializableContent.CreateContentPack();

            var loadDispatchers = new List<Action>();
            PopulateAssetLoadDispatchers(loadDispatchers, contentPack);

            for (var i = 0; i < loadDispatchers.Count; i++)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap(i + 1, 0, loadDispatchers.Count, 0.9f, 0.99f));
                yield return null;
            }

            contentPack.entityStateTypes.Add(Assembly.GetExecutingAssembly().GetTypes().Where(el => typeof(EntityState).IsAssignableFrom(el)).ToArray());

            args.ReportProgress(1F);
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        private IEnumerable HandleAsyncOperationProgress(LoadStaticContentAsyncArgs args, AsyncOperation operation, float startProgress, float endProgress)
        {
            args.ReportProgress(startProgress);
            yield return null;

            while (!operation.isDone)
            {
                args.ReportProgress(Util.Remap(operation.progress, 0, 1, startProgress, endProgress));
                yield return null;
            }
            args.ReportProgress(endProgress);
        }
    }
}