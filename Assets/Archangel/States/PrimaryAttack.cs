using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityStates;
using RoR2.Skills;
using UnityEngine.Networking;
using RoR2;
using System;
using System.Linq;

namespace Archangel.States
{
    public class PrimaryAttack : ArchangelBaseAttack
    {
        [SerializeField]
        public static EntityStateConfigurationCollection stepConfigurations;
        public override EntityStateConfigurationCollection StepConfigurations => stepConfigurations;

        private static Action<object>[] _stepInitActions;
        protected override Action<object>[] StepInitActions { get => _stepInitActions; set => _stepInitActions = value; }
        protected override string PlaybackRateParameter => "PrimaryPlaybackRate";
    }
}