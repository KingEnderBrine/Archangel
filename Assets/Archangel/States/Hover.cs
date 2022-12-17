using EntityStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel.States
{
    public class Hover : EntityState
    {
        [SerializeField]
        public Vector3 velocity;

        public float hoverDuration;

        public override void OnEnter()
        {
            base.OnEnter();

            if (!isAuthority)
            {
                return;
            }

            if (characterMotor)
            {
                characterMotor.velocity = velocity;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
            {
                return;
            }

            if (characterMotor)
            {
                characterMotor.velocity = velocity;
            }

            if (inputBank.moveVector.magnitude > 0.25F || fixedAge >= hoverDuration)
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
