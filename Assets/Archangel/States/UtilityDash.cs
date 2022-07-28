using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel.States
{
    public class UtilityDash : BaseSkillState
    {
        [SerializeField]
        public float dashSpeed;
        [SerializeField]
        public float dashDuration;

        private Vector3 startDashVector;
        public ArchangelUtilityProjectileBehaviour projectileBehaviour;

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.PrioritySkill;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body, Override", "Armature|ability3", "UtilityPlacybackRate", dashDuration);
            if (isAuthority)
            {
                startDashVector = (projectileBehaviour.transform.position - transform.position).normalized;
                StartAimMode(new Ray(transform.position, startDashVector), dashDuration, true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority && projectileBehaviour)
            {
                Destroy(projectileBehaviour.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                characterMotor.velocity = Vector3.zero;
                var dashVector = (projectileBehaviour.transform.position - transform.position);
                if (fixedAge >= dashDuration || Vector3.Dot(startDashVector, dashVector) < 0)
                {
                    if (Physics.Raycast(transform.position, Vector3.down, 0.5F, LayerIndex.world.mask))
                    {
                        return;
                    }
                    if (activatorSkillSlot.stock == 0)
                    {
                        characterMotor.velocity = (Vector3.up + Vector3.ProjectOnPlane(startDashVector, Vector3.up).normalized) * 10;
                        outer.SetNextStateToMain();
                    }
                    else
                    {
                        outer.SetNextState(new UtilityAfterDash());
                    }
                    return;
                }
                characterMotor.rootMotion += Vector3.ClampMagnitude((dashVector.normalized * dashSpeed - Vector3.up * Physics.gravity.y) * Time.fixedDeltaTime, dashVector.magnitude);
            }
        }
    }
}
