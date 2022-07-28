using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class SwordPrintController : PrintController
    {
        public enum PrintDirection { BottomUp = 0, TopDown = 1, BackToFront = 3 };

        private static readonly Dictionary<GameObject, PrintInfo> printInfos = new Dictionary<GameObject, PrintInfo>();
        private static int printDirectionPropertyId = Shader.PropertyToID("_PrintDirection");

        public string controllerName;
        public PrintDirection direction;
        public bool disableGameObjectWhenFinished;

        private ItemDisplay itemDisplay;
        private Renderer[] renderers;

        private new void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            //characterModel = GetComponentInParent<CharacterModel>();
            itemDisplay = GetComponentInParent<ItemDisplay>();
            renderers = GetComponentsInChildren<Renderer>();

            printInfos.GetOrCreate(gameObject, () => new PrintInfo()).counter++;
        }

        private new void OnEnable()
        {
            Setup();

            foreach (var pair in rendererMaterialPairs)
            {
                pair.material.SetInt(printDirectionPropertyId, (int)direction);
            }

            base.OnEnable();
        }

        private new void Update()
        {
            var wasEnabled = enabled;
            base.Update();

            if (wasEnabled != enabled && disableGameObjectWhenFinished)
            {
                gameObject.SetActive(false);
            }
        }

        private new void OnDestroy()
        {
            if (printInfos.TryGetValue(gameObject, out var printInfo) && --printInfo.counter <= 0)
            {
                printInfos.Remove(gameObject);
            }
        }

        private void Setup()
        {
#if UNITY_EDITOR
            if (!printShader)
            {
                printShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>("Assets/ExternalReferenceAssets/ror2-base-shaders_shader_assets_all/shaders/deferred/hgstandard.asset");
            }
#endif
            var printInfo = printInfos[gameObject];

            if (itemDisplay && printInfo.baseRendererInfos != itemDisplay.rendererInfos)
            {
                printInfo.baseRendererInfos = itemDisplay.rendererInfos;

                var pairs = new List<RendererMaterialPair>();
                for (var i = 0; i < renderers.Length; ++i)
                {
                    var index = -1;
                    for (var j = 0; j < printInfo.baseRendererInfos.Length; j++)
                    {
                        if (printInfo.baseRendererInfos[j].renderer == renderers[i])
                        {
                            index = j;
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        continue;
                    }

                    ref var local = ref printInfo.baseRendererInfos[index];
                    if (local.renderer && local.renderer.material.shader == printShader)
                    {
                        pairs.Add(new RendererMaterialPair(local.renderer, local.renderer.material));
                    }
                }
                printInfo.pairs = pairs.ToArray();
            }
            else if (!printInfo.withoutInfo)
            {
                var pairs = new List<RendererMaterialPair>();
                foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
                {
                    pairs.Add(new RendererMaterialPair(renderer, renderer.material));
                }
                printInfo.pairs = pairs.ToArray();
                printInfo.withoutInfo = true;
            }

            if (rendererMaterialPairs != printInfo.pairs)
            {
                rendererMaterialPairs = printInfo.pairs;
            }
        }

        private class PrintInfo
        {
            public int counter;
            public RendererMaterialPair[] pairs;
            public CharacterModel.RendererInfo[] baseRendererInfos;
            public bool withoutInfo;
        }
    }
}
