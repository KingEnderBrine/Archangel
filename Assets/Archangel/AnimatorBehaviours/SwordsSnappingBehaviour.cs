using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

namespace Archangel.AnimatorBehaviours
{
    public class SwordsSnappingBehaviour : StateMachineBehaviour
    {
        public enum SnapBehaviour { None, Snap, UnSnap };

        public SnapBehaviour onEnter = SnapBehaviour.Snap;
        public float onEnterDelay;
        public SnapBehaviour onExit = SnapBehaviour.UnSnap;
        public float onExitDelay;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ProcessBehaviour(animator, onEnter, onEnterDelay);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ProcessBehaviour(animator, onExit, onExitDelay);
        }

        private void ProcessBehaviour(Animator animator, SnapBehaviour snapBehaviour, float delay)
        {
            if (snapBehaviour == SnapBehaviour.Snap)
            {
                var followerLocator = animator.GetComponent<FollowerLocator>();
                followerLocator.follower.Snap(delay);
            }
            else if (snapBehaviour == SnapBehaviour.UnSnap)
            {
                var followerLocator = animator.GetComponent<FollowerLocator>();
                followerLocator.follower.UnSnap(delay);
            }
        }
    }
}
