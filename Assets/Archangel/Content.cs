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
        private const string bundleName = "ArchangelTeam_Archangel";
        
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
            assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ArchangelPlugin.Instance.Info.Location), bundleName));
            args.ReportProgress(0.01f);
            var serializableContent = assetBundle.LoadAsset<SerializableContentPack>(ContentPackPath);
            contentPack = serializableContent.CreateContentPack();

            var loadDispatchers = new List<Action>();
            PopulateAssetLoadDispatchers(loadDispatchers, contentPack);

            contentPack.entityStateTypes.Add(Assembly.GetExecutingAssembly().GetTypes().Where(el => typeof(EntityState).IsAssignableFrom(el)).ToArray());

            for (var i = 0; i < loadDispatchers.Count; i++)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap(i + 1, 0, loadDispatchers.Count, 0.05f, 0.98f));
                yield return null;
            }

            args.ReportProgress(0.99F);
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
    }
}