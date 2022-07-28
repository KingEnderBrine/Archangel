using EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archangel
{
    public class ComboSkillDef : SteppedSkillDef
    {
        public bool resetStepsIfWasInAnotherState = true;
        public Sprite[] spritesForSteps;

        public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
        {
            if (skillSlot.stateMachine.state.GetType() != activationState.stateType)
            {
                var data = skillSlot.skillInstanceData as InstanceData;
                data.step = 0;
            }

            return base.InstantiateNextState(skillSlot);
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var data = base.OnAssigned(skillSlot);
            if (data is InstanceData instanceData)
            {
                instanceData.step = -1;
            }

            return data;
        }

        public override Sprite GetCurrentIcon([NotNull] GenericSkill skillSlot)
        {
            if (skillSlot.skillInstanceData is InstanceData data && data.step >= 0 && data.step < spritesForSteps.Length)
            {
                return HG.ArrayUtils.GetSafe(spritesForSteps, data.step) ?? base.GetCurrentIcon(skillSlot);
            }

            return base.GetCurrentIcon(skillSlot);
        }
    }
}