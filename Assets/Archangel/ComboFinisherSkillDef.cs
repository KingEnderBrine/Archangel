using EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace Archangel
{
    public class ComboFinisherSkillDef : SkillDef
    {
        public SkillSlot interruptSkillSlot;

        public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
        {
            var entityState = base.InstantiateNextState(skillSlot);
            var skillLocator = skillSlot.GetComponent<SkillLocator>();
            var interruptSkill = skillLocator.GetSkill(interruptSkillSlot);
            if (!interruptSkill || !(interruptSkill.skillDef is SteppedSkillDef interruptSkillDef) || !(skillSlot.stateMachine.state.GetType() == interruptSkill.activationState.stateType))
            {
                return entityState;
            }

            var interruptSkillInstanceData = interruptSkill.skillInstanceData as SteppedSkillDef.InstanceData;
            if (entityState is SteppedSkillDef.IStepSetter stepSetter)
            {
                stepSetter.SetStep((int)Mathf.Repeat(interruptSkillInstanceData.step - 1, interruptSkillDef.stepCount));
            }
            return entityState;
        }
    }
}
