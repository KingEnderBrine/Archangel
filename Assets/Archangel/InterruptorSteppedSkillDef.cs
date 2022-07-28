using EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archangel
{
    public class InterruptorSteppedSkillDef : SteppedSkillDef
    {
        public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
        {
            var state = base.InstantiateNextState(skillSlot);
            if (state is ISkillInterruptor interruptor && skillSlot.stateMachine.mainStateType.stateType != skillSlot.stateMachine.state.GetType())
            {
                interruptor.SetInterruptedState(skillSlot.stateMachine.state);
            }
            return state;
        }
    }
}
