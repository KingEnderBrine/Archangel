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

            var swordsController = cachedModelTransform.GetComponent<SwordsLocator>().swordsController;
            var swordsAnimator = swordsController.GetComponent<Animator>();
            var swordsRagdoll = swordsController.GetComponent<RagdollController>();

            var force = Vector3.up * 3f;
            if (characterMotor)
            {
                force += characterMotor.velocity;
            }

            PlayAnimationOnAnimator(swordsAnimator, "Base", "Default");
            swordsRagdoll.BeginRagdoll(force);
        }
    }
}
