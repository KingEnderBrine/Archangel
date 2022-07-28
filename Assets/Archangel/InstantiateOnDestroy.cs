using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class InstantiateOnDestroy : MonoBehaviour
    {
        public GameObject prefab;

        private void OnDestroy()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }
            if (!prefab)
            {
                ArchangelPlugin.InstanceLogger.LogWarning($"({nameof(InstantiateOnDestroy)}) invalid prefab");
                return;
            }
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}
