using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel.States
{
    public class DeathState : EntityStates.Commando.DeathState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            var swordsObject = cachedModelTransform.GetComponent<SwordsLocator>().instance;
            var swordsRagdoll = swordsObject.GetComponent<RagdollController>();

            var force = Vector3.up * 3f;
            if (characterMotor)
            {
                force += characterMotor.velocity;
            }

            swordsRagdoll.BeginRagdoll(force);
        }
    }
}
