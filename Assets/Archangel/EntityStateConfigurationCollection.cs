using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    [CreateAssetMenu]
    public class EntityStateConfigurationCollection : ScriptableObject, IEnumerable<EntityStateConfiguration>
    {
        [SerializeField]
        private List<EntityStateConfiguration> collection;

        public EntityStateConfiguration this[int index]
        {
            get { return collection[index]; }
            set { collection[index] = value; }
        }

        public IEnumerator<EntityStateConfiguration> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
