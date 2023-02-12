using BepInEx;
using BepInEx.Logging;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Archangel
{
    //[BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, Name, Version)]
    public class ArchangelPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "Paladin_Alliance.Archangel";
        public const string Name = "Archangel";
        public const string Version = "0.0.6";

        internal static ArchangelPlugin Instance { get; private set; }
        internal static ManualLogSource InstanceLogger => Instance ? Instance.Logger : null;

        private void Awake()
        {
            Instance = this;
            ContentManager.collectContentPackProviders += Content.CollectProvider;
        }
    }
}