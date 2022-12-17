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
    public class DoNotInterruptSelfSteppedSkillDef : SteppedSkillDef
    {
        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return skillSlot.activationState.stateType != skillSlot.stateMachine.state.GetType() && base.CanExecute(skillSlot);
        }
    }
}
