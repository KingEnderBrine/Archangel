using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class Follower : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        public float positionDampTime = 0.2f;
        public bool positionWithNoise;
        public float noiseScale = 0.01f;
        public float noiseValueScale = 0.05f;
        [Range(0, 10)]
        public float rotationSpeed = 5;
        public float snapDuration = 0.1f;

        private bool snapped;
        private float snappingTime = -1;
        private Vector3 velocity;
        private float noiseOffset;
        private float age;
        public float lastTime;
        public float lastNoise;

        private void Awake()
        {
            noiseOffset = UnityEngine.Random.Range(0, 1);
        }

        private void Update()
        {
            if (!target)
            {
                return;
            }

            var targetPosition = target.position + target.rotation * offset;
            if (snapped)
            {
                transform.rotation = target.rotation;
                transform.position = targetPosition;
            }
            else if (snappingTime >= 0)
            {
                snappingTime += Time.deltaTime;
                var t = Mathf.Clamp(snappingTime, 0, snapDuration) * (1 / snapDuration);
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, t);
                transform.position = Vector3.Lerp(transform.position, targetPosition, t);
                if (snappingTime > snapDuration)
                {
                    snapped = true;
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Mathf.Clamp01(rotationSpeed * Time.deltaTime));
                lastTime = positionDampTime;
                if (positionWithNoise)
                {
                    lastNoise = Mathf.PerlinNoise(noiseOffset, age * noiseScale);
                    lastTime += lastNoise * noiseValueScale;
                }
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, lastTime);
            }

            age += Time.deltaTime;
        }

        public void Snap()
        {
            snappingTime = 0;
            velocity = Vector3.zero;
        }

        public void UnSnap()
        {
            snapped = false;
            snappingTime = -1;
        }
    }
}
