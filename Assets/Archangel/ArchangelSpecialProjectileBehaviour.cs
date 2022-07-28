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
    public class ArchangelSpecialProjectileBehaviour : MonoBehaviour, IProjectileImpactBehavior
    {
        private OverlapAttack overlapAttack;
        private Rigidbody rigidBody;
        private new Collider collider;
        private ProjectileDamage projectileDamage;
        private ProjectileController projectileController;
        private float fixedAge;
        private float ageAfterImpact;
        private bool hadImpact;
        private Vector3 targetCenterOffset;
        private float distanceToBreakFree;

        public static event Action<ArchangelSpecialProjectileBehaviour> ProjectileCreated;

        [NonSerialized]
        public HealthComponent target;

        public float maxLifetime;
        public float lifetimeAfterImpact;
        public float startMovementDelay;
        public float speed;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            
            ProjectileCreated?.Invoke(this);
        }

        private void Start()
        {
            var ownerBody = projectileController.owner.GetComponent<CharacterBody>();
            overlapAttack = new OverlapAttack
            {
                attacker = projectileController.owner,
                inflictor = projectileController.owner,
                damage = projectileDamage.damage,
                hitBoxGroup = GetComponent<HitBoxGroup>(),
                isCrit = projectileDamage.crit,
                procCoefficient = projectileController.procCoefficient,
                procChainMask = projectileController.procChainMask,
                damageColorIndex = projectileDamage.damageColorIndex,
                damageType = projectileDamage.damageType,
                teamIndex = ownerBody.teamComponent.teamIndex
            };
            if (target)
            {
                targetCenterOffset = target.body.corePosition - target.transform.position;
                distanceToBreakFree = Mathf.Max(target.body.radius, 2);
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            fixedAge += Time.fixedDeltaTime;

            if (fixedAge < startMovementDelay)
            {
                return;
            }

            if (hadImpact)
            {
                ageAfterImpact += Time.fixedDeltaTime;
                if (ageAfterImpact > lifetimeAfterImpact)
                {
                    Destroy(gameObject);
                }
                return;
            }

            rigidBody.velocity = transform.forward * speed;

            if (target && target.alive)
            {
                PullTarget();
            }

            overlapAttack.Fire();

            if (fixedAge > maxLifetime + startMovementDelay)
            {
                Destroy(gameObject);
            }
        }

        private void PullTarget()
        {
            var position = transform.position - targetCenterOffset;
            if (Vector3.Distance(target.transform.position, position) >= distanceToBreakFree)
            {
                target = null;
                return;
            }

            var characterMotor = target.body.characterMotor;
            var rigidbody = target.body.rigidbody;
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.Motor.MoveCharacter(position);
            }
            else if (rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.MovePosition(position);
            }
            else
            {
                target.transform.position = position;
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            Destroy(rigidBody);
            Destroy(collider);
            hadImpact = true;
        }
    }
}
