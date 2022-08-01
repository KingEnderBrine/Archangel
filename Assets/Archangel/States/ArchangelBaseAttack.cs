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
        protected Follower swordsFollower;

        protected abstract string PlaybackRateParameter { get; } 

        public override void OnEnter()
        {
            LoadStepConfiguration();

            swordsController = outer.commonComponents.modelLocator.modelTransform.GetComponent<SwordsLocator>().ControllerInstance;
            swordsFollower = swordsController.GetComponent<Follower>();
            swordsFollower.Snap();

            durationBeforeInterruptable = baseDurationBeforeInterruptable / attackSpeedStat;

            base.OnEnter();

            duration += fixedAdditionalDuration;
        }

        public override void OnExit()
        {
            swordsFollower.UnSnap();
            animator.SetBool("Waiting", false);
            GetModelAnimator().SetBool("Waiting", false);
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType = (DamageType)damageType;
        }

        public override void BeginMeleeAttackEffect()
        {
            base.BeginMeleeAttackEffect();
            cameraTargetParams.targetRecoil += recoilDirection.normalized * UnityEngine.Random.Range(minAmplitudeMultiplier, maxAmplitudeMultiplier);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return durationBeforeInterruptable > fixedAge ? InterruptPriority.PrioritySkill : InterruptPriority.Skill;
        }

        public override void PlayAnimation()
        {
            if (!string.IsNullOrWhiteSpace(archangelAnimationLayer) && !string.IsNullOrWhiteSpace(archangelAnimationStateName))
            {
                animator.SetBool("Waiting", true);
                PlayAnimation(archangelAnimationLayer, archangelAnimationStateName, PlaybackRateParameter, duration);
            }

            animator = swordsController.GetComponent<Animator>();

            if (!string.IsNullOrWhiteSpace(swordsAnimationLayer) && !string.IsNullOrWhiteSpace(swordsAnimationStateName))
            {
                animator.SetBool("Waiting", true);
                PlayAnimationOnAnimator(animator, swordsAnimationLayer, swordsAnimationStateName, PlaybackRateParameter, duration);
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
    }
}
