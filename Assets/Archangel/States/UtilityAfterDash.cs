using EntityStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel.States
{
    public class UtilityAfterDash : BaseState
    {
        [SerializeField]
        public float hoverDelay;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (characterMotor)
                {
                    characterMotor.velocity = Vector3.zero;
                }

                if (inputBank.moveVector.magnitude > 0.25F || fixedAge >= hoverDelay)
                {
                    outer.SetNextStateToMain();
                }
            }
        }
    }
}
