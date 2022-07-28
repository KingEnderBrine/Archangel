using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class DetachFromParent : MonoBehaviour
    {
        private enum When { Awake, Start }

        [SerializeField]
        private When when;
        [SerializeField]
        private bool worldPositionStays = true;


        private void Awake()
        {
            if (when == When.Awake)
            {
                transform.SetParent(null, worldPositionStays);
            }
        }

        private void Start()
        {
            if (when == When.Start)
            {
                transform.SetParent(null, worldPositionStays);
            }
        }
    }
}
