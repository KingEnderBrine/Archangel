using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public static class Utilities
    {
        public const float crossfadeDuration = 0.1F;
        
        public static void PlayCrossfadeOnAnimator(
            Animator animator,
            string layerName,
            string animationStateName,
            float crossfadeDuration)
        {
            animator.speed = 1f;
            animator.Update(0.0f);

            var layerIndex = animator.GetLayerIndex(layerName);
            animator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
            //Do a very small offset in the animation, so that the curve in mecanimHitboxActiveParameter
            //would mix in crossfade transition and go bellow 0.5 when you interrupt during the attack
            //because otherwise the attack would trigger on the first frame of the state.
            animator.Update(0.001f);
        }

        public static void PlayCrossfadeOnAnimator(
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
