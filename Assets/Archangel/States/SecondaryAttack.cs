using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel.States
{
    public class SecondaryAttack : ArchangelBaseAttack
    {
        [SerializeField]
        public static EntityStateConfigurationCollection stepConfigurations;
        public override EntityStateConfigurationCollection StepConfigurations => stepConfigurations;

        private static Action<object>[] _stepInitActions;
        protected override Action<object>[] StepInitActions { get => _stepInitActions; set => _stepInitActions = value; }
    }
}