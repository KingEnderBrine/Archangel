using EntityStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archangel
{
    public interface ISkillInterruptor
    {
        void SetInterruptedState(EntityState entityState);
    }
}
