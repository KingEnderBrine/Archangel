using EntityStates;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel.States
{
    public class UtilityFire : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        [SerializeField]
        public static GameObject projectilePrefab;

        [SerializeField]
        public float damageCoefficient;
        [SerializeField]
        public float animationDuration;
        [SerializeField]
        public float minimumDuration;

        private byte step;
        private bool keyWasUp;
        private ArchangelUtilityProjectileBehaviour projectileBehaviour;
        private bool dashing;
        private EntityStateMachine hoverStateMachine;

        public override InterruptPriority GetMinimumInterruptPriority() => fixedAge < minimumDuration ? InterruptPriority.Vehicle : InterruptPriority.Skill;

        public override void OnEnter()
        {
            base.OnEnter();

            var swordsController = outer.commonComponents.modelLocator.modelTransform.GetComponent<SwordsLocator>().swordsController;
            var swordsAnimator = swordsController.GetComponent<Animator>();

            Utilities.PlayCrossfadeOnAnimator(GetModelAnimator(), "Gesture, Override", "Skill3", "UtilityPlacybackRate", animationDuration, Utilities.crossfadeDuration);
            Utilities.PlayCrossfadeOnAnimator(swordsAnimator, "Base", "Default", Utilities.crossfadeDuration);

            if (isAuthority)
            {
                var ray = GetAimRay();
                var prefabBehaviour = projectilePrefab.GetComponent<ArchangelUtilityProjectileBehaviour>();
                var endPoint = ray.origin + ray.direction * (prefabBehaviour.speed * prefabBehaviour.maxLifetime);

                var projectilePosition = ray.origin + Quaternion.LookRotation(ray.direction) * (Vector3.left * step * 2 - Vector3.left);
                var projectileRotation = Quaternion.LookRotation((endPoint - projectilePosition).normalized) * Quaternion.AngleAxis(180 * step - 90, Vector3.forward);
                
                ArchangelUtilityProjectileBehaviour.ProjectileCreated += ModifyProjectile;
                ProjectileManager.instance.FireProjectile(projectilePrefab, projectilePosition, projectileRotation, gameObject, damageStat * damageCoefficient, 0, RollCrit());

                hoverStateMachine = EntityStateMachine.FindByCustomName(gameObject, "Hover");
                hoverStateMachine.SetNextState(new Hover { hoverDuration = projectileBehaviour.delayBeforeStart + projectileBehaviour.maxLifetime + projectileBehaviour.lifetimeAfterImpact });
            }
            void ModifyProjectile(ArchangelUtilityProjectileBehaviour behaviour)
            {
                ArchangelUtilityProjectileBehaviour.ProjectileCreated -= ModifyProjectile;
                projectileBehaviour = behaviour;
                behaviour.step = step;
                StartAimMode(behaviour.delayBeforeStart + behaviour.maxLifetime, true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority && projectileBehaviour && !dashing)
            {
                Destroy(projectileBehaviour.gameObject);
            }
        }

        public override void Update()
        {
            base.Update();

            if (!isAuthority)
            {
                return;
            }
            if (!projectileBehaviour)
            {
                return;
            }

            var keyDown = IsKeyDownAuthority();
            if (keyWasUp && keyDown)
            {
                this.ClaimButtonPressAuthority(skillLocator, inputBank);
                var nextState = new UtilityDash
                {
                    projectileBehaviour = projectileBehaviour,
                    activatorSkillSlot = activatorSkillSlot,
                };
                outer.SetNextState(nextState);
                dashing = true;
                return;
            }

            keyWasUp = !keyDown;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (!projectileBehaviour)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            step = reader.ReadByte();
        }

        public void SetStep(int step)
        {
            this.step = (byte)step;
        }
    }
}