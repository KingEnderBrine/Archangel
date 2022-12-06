using EntityStates;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel.States
{
    public class UtilityFire : BaseSkillState, SteppedSkillDef.IStepSetter, ISkillInterruptor
    {
        [SerializeField]
        public static GameObject projectilePrefab;

        [SerializeField]
        public float damageCoefficient;
        [SerializeField]
        public float animationDuration;

        private byte step;
        private bool keyWasUp;
        private ArchangelUtilityProjectileBehaviour projectileBehaviour;
        private bool dashing;
        private bool holdInAir;

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayAnimation("Gesture, Override", "Skill3", "UtilityPlacybackRate", animationDuration);
            if (isAuthority)
            {
                var ray = GetAimRay();
                var prefabBehaviour = projectilePrefab.GetComponent<ArchangelUtilityProjectileBehaviour>();
                var endPoint = ray.origin + ray.direction * (prefabBehaviour.speed * prefabBehaviour.maxLifetime);

                var projectilePosition = ray.origin + Quaternion.LookRotation(ray.direction) * (Vector3.left * step * 2 - Vector3.left);
                var projectileRotation = Quaternion.LookRotation((endPoint - projectilePosition).normalized) * Quaternion.AngleAxis(180 * step - 90, Vector3.forward);
                
                ArchangelUtilityProjectileBehaviour.ProjectileCreated += ModifyProjectile;
                ProjectileManager.instance.FireProjectile(projectilePrefab, projectilePosition, projectileRotation, gameObject, damageStat * damageCoefficient, 0, RollCrit());
            }
            void ModifyProjectile(ArchangelUtilityProjectileBehaviour behaviour)
            {
                ArchangelUtilityProjectileBehaviour.ProjectileCreated -= ModifyProjectile;
                projectileBehaviour = behaviour;
                behaviour.step = step;
                StartAimMode(behaviour.maxLifetime, true);
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
            if (isAuthority)
            {
                if (!projectileBehaviour)
                {
                    return;
                }
                if (holdInAir && inputBank.moveVector.magnitude > 0.25F)
                {
                    holdInAir = false;
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
                else
                {
                    keyWasUp = !keyDown;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (holdInAir && characterMotor)
                {
                    characterMotor.velocity = Vector3.zero;
                }

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

        public void SetInterruptedState(EntityState entityState)
        {
            holdInAir = entityState is UtilityAfterDash;
        }
    }
}