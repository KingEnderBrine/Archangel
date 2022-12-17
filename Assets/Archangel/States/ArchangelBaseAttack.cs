using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel.States
{
    public abstract class ArchangelBaseAttack : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {
        protected abstract Action<object>[] StepInitActions { get; set; }
        public abstract EntityStateConfigurationCollection StepConfigurations { get; }

        [SerializeField]
        public float baseDurationBeforeInterruptable;
        [SerializeField]
        public float fixedAdditionalDuration;
        [SerializeField]
        public bool setWaitingParameter;
        [SerializeField]
        public string archangelAnimationLayer;
        [SerializeField]
        public string archangelAnimationStateName;
        [SerializeField]
        public string swordsAnimationLayer;
        [SerializeField]
        public string swordsAnimationStateName;
        [SerializeField]
        public Vector2 recoilDirection;
        [SerializeField]
        public float minAmplitudeMultiplier;
        [SerializeField]
        public float maxAmplitudeMultiplier;
        [SerializeField]
        [EnumMask(typeof(DamageType))]
        public int damageType;

        private int step = -1;
        private float durationBeforeInterruptable;
        private Animator archangelAnimator;
        private Animator swordsAnimator;

        protected SwordsController swordsController;

        protected abstract string PlaybackRateParameter { get; } 

        public override void OnEnter()
        {
            LoadStepConfiguration();

            swordsController = outer.commonComponents.modelLocator.modelTransform.GetComponent<SwordsLocator>().swordsController;
            swordsAnimator = swordsController.GetComponent<Animator>();

            base.OnEnter();

            archangelAnimator = animator;
            durationBeforeInterruptable = baseDurationBeforeInterruptable / attackSpeedStat;
        }

        public override void OnExit()
        {
            if (setWaitingParameter)
            {
                animator.SetBool("Waiting", false);
                GetModelAnimator().SetBool("Waiting", false);
            }
            base.OnExit();
        }

        public override float CalcDuration()
        {
            return base.CalcDuration() + fixedAdditionalDuration;
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType = (DamageType)damageType;
        }

        public override void BeginMeleeAttackEffect()
        {
            if (this.meleeAttackStartTime != Run.FixedTimeStamp.positiveInfinity)
            {
                return;
            }

            base.BeginMeleeAttackEffect();

            AddRecoil(
                recoilDirection.normalized.x * minAmplitudeMultiplier,
                recoilDirection.normalized.x * maxAmplitudeMultiplier,
                recoilDirection.normalized.y * minAmplitudeMultiplier,
                recoilDirection.normalized.y * maxAmplitudeMultiplier);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return durationBeforeInterruptable > fixedAge ? InterruptPriority.PrioritySkill : InterruptPriority.Skill;
        }

        public override void PlayAnimation()
        {
            var animationDuration = duration - fixedAdditionalDuration;
            if (!string.IsNullOrWhiteSpace(archangelAnimationLayer) && !string.IsNullOrWhiteSpace(archangelAnimationStateName))
            {
                if (setWaitingParameter)
                {
                    animator.SetBool("Waiting", true);
                }
                Utilities.PlayCrossfadeOnAnimator(animator, archangelAnimationLayer, archangelAnimationStateName, PlaybackRateParameter, animationDuration, Utilities.crossfadeDuration / attackSpeedStat);
            }

            animator = swordsAnimator;

            if (!string.IsNullOrWhiteSpace(swordsAnimationLayer) && !string.IsNullOrWhiteSpace(swordsAnimationStateName))
            {
                if (setWaitingParameter)
                {
                    animator.SetBool("Waiting", true);
                }
                Utilities.PlayCrossfadeOnAnimator(animator, swordsAnimationLayer, swordsAnimationStateName, PlaybackRateParameter, animationDuration, Utilities.crossfadeDuration / attackSpeedStat);
            }
        }

        public override void OnMeleeHitAuthority()
        {
            archangelAnimator.speed = 0;
            duration += hitPauseTimer;
        }

        public override void AuthorityExitHitPause()
        {
            base.AuthorityExitHitPause();
            archangelAnimator.speed = 1;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            step = reader.ReadInt32();
        }

        private void LoadStepConfiguration()
        {
            if (step < 0)
            {
                return;
            }

            if (StepInitActions == null)
            {
                StepInitActions = StepConfigurations.Select(el => el ? el.BuildInstanceInitializer() : null).ToArray();
            }

            HG.ArrayUtils.GetSafe(StepInitActions, step)?.Invoke(this);
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
