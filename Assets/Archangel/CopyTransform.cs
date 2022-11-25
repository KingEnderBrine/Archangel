using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archangel
{
    public class CopyTransform : MonoBehaviour
    {
        public Transform target;

        private void LateUpdate()
        {
            if (target)
            {
                transform.rotation = target.rotation;
                transform.position = target.position;
            }
        }
    }
}