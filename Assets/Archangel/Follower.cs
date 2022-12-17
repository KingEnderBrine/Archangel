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
    public class Follower : MonoBehaviour
    {
        public AimAnimator aimAnimator;
        public Vector3 aimRotationOffset;

        public Transform target;
        public Vector3 offset;
        public float positionDampTime = 0.2f;
        public bool positionWithNoise;
        public float noiseScale = 0.01f;
        public float noiseValueScale = 0.05f;
        [Range(0, 10)]
        public float rotationSpeed = 5;
        public float snapDuration = 0.1f;
        public float immediateSnapRadius = 0.01f;
        public float maxDistanceForRatation = 3f;

        private bool snapped;
        private float snappingTime = -1;
        private Vector3 velocity;
        private float noiseOffset;
        private float age;
        private float lastTime;
        private float lastNoise;
        private IEnumerator snapCoroutine;
        private IEnumerator unSnapCoroutine;

        private void Awake()
        {
            noiseOffset = UnityEngine.Random.Range(0, 1);
        }

        private void LateUpdate()
        {
            if (!target)
            {
                return;
            }

            var targetPosition = target.position + target.rotation * offset;
            var targetRotation = target.rotation;
            if (aimAnimator)
            {
                targetRotation *= Quaternion.Slerp(Quaternion.AngleAxis(aimAnimator.currentLocalAngles.pitch, Vector3.right) * Quaternion.Euler(aimRotationOffset), Quaternion.identity, Mathf.Clamp01((transform.position - targetPosition).magnitude / maxDistanceForRatation));
            }

            if (snapped)
            {
                transform.rotation = targetRotation;
                transform.position = targetPosition;
            }
            else if (snappingTime >= 0)
            {
                snappingTime += Time.deltaTime;
                var t = Mathf.Clamp(snappingTime, 0, snapDuration) * (1 / snapDuration);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
                transform.position = Vector3.Lerp(transform.position, targetPosition, t);
                if (snappingTime > snapDuration)
                {
                    snapped = true;
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Clamp01(rotationSpeed * Time.deltaTime));
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

        public void Snap(float delay = 0)
        {
            if (delay == 0)
            {
                SnapCore();
            }
            else if (snapCoroutine == null)
            {
                StartCoroutine(snapCoroutine = SnapCoroutine(delay));
            }

            if (unSnapCoroutine != null)
            {
                StopCoroutine(unSnapCoroutine);
                unSnapCoroutine = null;
            }
        }

        private IEnumerator SnapCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SnapCore();
        }

        private void SnapCore()
        {
            snapCoroutine = null;

            var targetPosition = target.position + target.rotation * offset;
            if ((targetPosition - transform.position).magnitude <= immediateSnapRadius)
            {
                snapped = true;
            }
            else
            {
                snappingTime = 0;
            }
            velocity = Vector3.zero;
        }

        public void UnSnap(float delay = 0)
        {
            if (delay == 0)
            {
                UnSnapCore();
            }
            else if (unSnapCoroutine == null)
            {
                StartCoroutine(unSnapCoroutine = UnSnapCoroutine(delay));
            }

            if (snapCoroutine != null)
            {
                StopCoroutine(snapCoroutine);
                snapCoroutine = null;
            }
        }

        private IEnumerator UnSnapCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            UnSnapCore();
        }

        private void UnSnapCore()
        {
            unSnapCoroutine = null;
            snapped = false;
            snappingTime = -1;
        }
    }
}
