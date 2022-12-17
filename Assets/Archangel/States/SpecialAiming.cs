using EntityStates;
using RoR2;
using UnityEngine;

namespace Archangel.States
{
    public class SpecialAiming : BaseSkillState
    {
        [SerializeField]
        public static GameObject indicatorPrefab;

        [SerializeField]
        public float radius;
        [SerializeField]
        public float maxAimDistance;

        private GameObject indicatorInstance;
        private bool validAim;

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Vehicle;

        public override void OnEnter()
        {
            base.OnEnter();

            var swordsController = outer.commonComponents.modelLocator.modelTransform.GetComponent<SwordsLocator>().swordsController;
            var swordsAnimator = swordsController.GetComponent<Animator>();

            Utilities.PlayCrossfadeOnAnimator(GetModelAnimator(), "Gesture, Override", "Empty", Utilities.crossfadeDuration);
            Utilities.PlayCrossfadeOnAnimator(swordsAnimator, "Base", "Default", Utilities.crossfadeDuration);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (indicatorInstance)
            {
                Destroy(indicatorInstance);
            }
        }


        public override void Update()
        {
            base.Update();

            UpdateIndicator();
            if (isAuthority && !IsKeyDownAuthority())
            {
                if (validAim)
                {
                    outer.SetNextState(new SpecialLifting { targetPoint = indicatorInstance.transform.position });
                }
                else
                {
                    outer.SetNextStateToMain();
                    activatorSkillSlot.AddOneStock();
                }
            }
        }

        private void UpdateIndicator()
        {
            if (!indicatorInstance)
            {
                indicatorInstance = GameObject.Instantiate(indicatorPrefab);
                indicatorInstance.transform.localScale = new Vector3(radius, radius, radius) * 2;
            }
            validAim = Physics.Raycast(GetAimRay(), out var hitInfo, maxAimDistance, LayerIndex.world.mask);
            if (validAim)
            {
                indicatorInstance.transform.position = hitInfo.point;
            }
            indicatorInstance.SetActive(validAim);
        }
    }
}
