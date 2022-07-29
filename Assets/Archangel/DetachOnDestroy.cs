using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class DetachOnDestroy : MonoBehaviour
    {
        public Transform[] targets = Array.Empty<Transform>();
        public Behaviour[] enableComponents = Array.Empty<Behaviour>();
        public Behaviour[] disableComponents = Array.Empty<Behaviour>();

        private void OnDestroy()
        {
            foreach (var target in targets)
            {
                target.SetParent(null, true);
            }

            foreach (var component in enableComponents)
            {
                component.enabled = true;
            }

            foreach (var component in disableComponents)
            {
                component.enabled = false;
            }
        }
    }
}
