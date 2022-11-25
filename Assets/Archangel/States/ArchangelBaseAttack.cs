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
        private const float crossfadeDuration = 0.1F;

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

        protected SwordsController swordsController;

        protected abstract string PlaybackRateParameter { get; } 

        public override void OnEnter()
        {
            LoadStepConfiguration();

            swordsController = outer.commonComponents.modelLocator.modelTransform.GetComponent<SwordsLocator>().swordsController;

            base.OnEnter();

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
                PlayCrossfadeOnAnimator(animator, archangelAnimationLayer, archangelAnimationStateName, PlaybackRateParameter, animationDuration, crossfadeDuration / attackSpeedStat);
            }

            animator = swordsController.GetComponent<Animator>();

            if (!string.IsNullOrWhiteSpace(swordsAnimationLayer) && !string.IsNullOrWhiteSpace(swordsAnimationStateName))
            {
                if (setWaitingParameter)
                {
                    animator.SetBool("Waiting", true);
                }
                PlayCrossfadeOnAnimator(animator, swordsAnimationLayer, swordsAnimationStateName, PlaybackRateParameter, animationDuration, crossfadeDuration / attackSpeedStat);
            }
        }

        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            AuthorityExitHitPause();
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

        protected void PlayCrossfadeOnAnimator(
            Animator animator,
            string layerName,
            string animationStateName,
            string playbackRateParam,
            float duration,
            float crossfadeDuration)
        {
            animator.speed = 1f;
            animator.Update(0.0f);

            var layerIndex = animator.GetLayerIndex(layerName);
            animator.SetFloat(playbackRateParam, 1f);
            animator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
            //Do a very small offset in the animation, so that the curve in mecanimHitboxActiveParameter
            //would mix in crossfade transition and go bellow 0.5 when you interrupt during the attack
            //because otherwise the attack would trigger on the first frame of the state.
            animator.Update(0.001f);

            var length = animator.GetNextAnimatorStateInfo(layerIndex).length;
            animator.SetFloat(playbackRateParam, length / duration);
        }
    }
}
