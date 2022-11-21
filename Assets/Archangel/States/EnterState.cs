using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archangel.States
{
    public class EnterState : SpawnTeleporterState
    {
        public override void FixedUpdate()
        {
            var oldHasTeleported = hasTeleported;
            base.FixedUpdate();
            if (oldHasTeleported != hasTeleported)
            {
                //TeleportOutController.AddTPOutEffect(this.characterModel, 1f, 0.0f, this.duration);
            }
        }
    }
}
