using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel
{
    public class ArchangelUtilityProjectileBehaviour : NetworkBehaviour
    {
        private Rigidbody rigidBody;
        private new Collider collider;
        private ProjectileController projectileController;
        private ProjectileSingleTargetImpact projectileImpact;
        private SwordsController ownerSwordsVisibilityController;
        private float fixedAge;
        private float ageAfterImpact;

        public static event Action<ArchangelUtilityProjectileBehaviour> ProjectileCreated;

        public float delayBeforeStart;
        public float maxLifetime;
        public float lifetimeAfterImpact;
        public float speed;

        [SyncVar]
        [HideInInspector]
        public byte step;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            projectileController = GetComponent<ProjectileController>();
            projectileImpact = GetComponent<ProjectileSingleTargetImpact>();

            ProjectileCreated?.Invoke(this);
        }

        private void Start()
        {
            var ownerBody = projectileController.owner.GetComponent<CharacterBody>();

            ownerSwordsVisibilityController = ownerBody.modelLocator.modelTransform.GetComponent<SwordsLocator>().swordsController;

            switch (step)
            {
                case 0:
                    ownerSwordsVisibilityController.HideRight();
                    break;
                case 1:
                    ownerSwordsVisibilityController.HideLeft();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (ownerSwordsVisibilityController)
            {
                switch (step)
                {
                    case 0:
                        ownerSwordsVisibilityController.ShowRight();
                        break;
                    case 1:
                        ownerSwordsVisibilityController.ShowLeft();
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active && !projectileController.isPrediction)
            {
                return;
            }
            fixedAge += Time.fixedDeltaTime;

            if (!projectileImpact.alive)
            {
                if (rigidBody)
                {
                    Destroy(rigidBody);
                    Destroy(collider);
                    rigidBody = null;
                }

                ageAfterImpact += Time.fixedDeltaTime;
                if (ageAfterImpact > lifetimeAfterImpact)
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (fixedAge > delayBeforeStart)
            {
                rigidBody.velocity = transform.forward * speed;
            }

            if (fixedAge > maxLifetime + delayBeforeStart)
            {
                projectileImpact.alive = false;
            }
        }
    }
}
