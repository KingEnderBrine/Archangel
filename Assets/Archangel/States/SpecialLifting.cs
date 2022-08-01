using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Archangel.States
{
    public class SpecialLifting : BaseState
    {
        private enum StateAction { StartLiftUp, LiftUp, SpawnProjectiles, HoldInAir, Exit }

        [SerializeField]
        public static GameObject projectilePrefab;

        [SerializeField]
        public float damageCoefficient;
        [SerializeField]
        public float liftUpDuration;
        [SerializeField]
        public float holdInAirDuration;
        [SerializeField]
        public float radius;
        [SerializeField]
        public float procCoefficient;
        [SerializeField]
        public float forceMagnitude;
        [SerializeField]
        public float distanceAboveGround;

        public Vector3 targetPoint;

        private readonly List<EnemyInfo> enemies = new List<EnemyInfo>();
        private StateAction currentAction;
        private float actionAge;

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Vehicle;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "Skill4", "SpecialPlaybackRate", (liftUpDuration + holdInAirDuration) * 1.5F, 0.1F);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(targetPoint);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            targetPoint = reader.ReadVector3();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active)
            {
                FixedUpdateServer();
            }

            if (fixedAge > liftUpDuration + holdInAirDuration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FixedUpdateServer()
        {
            actionAge += Time.fixedDeltaTime;
            ClearDeadEnemies();
            switch (currentAction)
            {
                case StateAction.StartLiftUp:
                    GatherEnemies();
                    currentAction = StateAction.LiftUp;
                    break;
                case StateAction.LiftUp:
                    MoveEnemiesToTargetPosition();
                    if (actionAge >= liftUpDuration)
                    {
                        actionAge = 0;
                        currentAction = StateAction.SpawnProjectiles;
                    }
                    break;
                case StateAction.SpawnProjectiles:
                    SpawnProjectiles();
                    UpdateCurrentEnemyPositions();
                    currentAction = StateAction.HoldInAir;
                    break;
                case StateAction.HoldInAir:
                    MoveEnemiesToTargetPosition();
                    if (actionAge >= holdInAirDuration)
                    {
                        actionAge = 0;
                        currentAction = StateAction.Exit;
                    }
                    break;
            }
        }

        private void ClearDeadEnemies()
        {
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                if (!enemies[i].healthComponent || !enemies[i].healthComponent.alive)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void MoveEnemiesToTargetPosition()
        {
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];

                var characterMotor = enemy.healthComponent.body.characterMotor;
                var rigidbody = enemy.healthComponent.body.rigidbody;
                var force = enemy.healthComponent.transform.position - enemy.targetPosition;
                var coefficient = 1f - Mathf.Clamp(force.magnitude / distanceAboveGround, 0.0f, 1f);
                force = force.normalized * this.forceMagnitude * (1f - coefficient);

                var velocity = Vector3.zero;
                var mass = 0F;
                if (characterMotor)
                {
                    velocity = characterMotor.velocity;
                    mass = characterMotor.mass;
                }
                else if (rigidbody)
                {
                    velocity = rigidbody.velocity;
                    mass = rigidbody.mass;
                }

                velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
                enemy.healthComponent.TakeDamageForce(force - velocity * (mass * coefficient), true);
            }
        }

        private void SpawnProjectiles()
        {
            foreach (var enemy in enemies)
            {
                var targetOffset = enemy.healthComponent.body.corePosition - enemy.healthComponent.transform.position;
                var projectilePosition = enemy.healthComponent.transform.position + targetOffset;
                var direction = (targetPoint - projectilePosition).normalized;
                ArchangelSpecialProjectileBehaviour.ProjectileCreated += ModifyProjectile;
                ProjectileManager.instance.FireProjectile(projectilePrefab, projectilePosition, Quaternion.LookRotation(direction), gameObject, damageStat * damageCoefficient, 0, RollCrit());

                void ModifyProjectile(ArchangelSpecialProjectileBehaviour behaviour)
                {
                    ArchangelSpecialProjectileBehaviour.ProjectileCreated -= ModifyProjectile;
                    behaviour.target = enemy.healthComponent;
                }
            }
        }

        private void GatherEnemies()
        {
            var search = new BullseyeSearch
            {
                filterByDistinctEntity = true,
                filterByLoS = false,
                maxDistanceFilter = radius,
                sortMode = BullseyeSearch.SortMode.None,
                viewer = characterBody,
                teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex),
                searchOrigin = targetPoint,
            };
            search.RefreshCandidates();
            foreach (var enemy in search.GetResults())
            {
                if (!enemy.healthComponent.body)
                {
                    continue;
                }
                var targetPosition = enemy.healthComponent.transform.position;
                var clearance = enemy.healthComponent.body.bestFitRadius + 1;
                var distanceToGround = distanceAboveGround;
                var distanceToCeiling = distanceAboveGround + clearance;

                if (Physics.Raycast(targetPosition, Vector3.down, out var hitInfo, distanceToGround, LayerIndex.world.mask))
                {
                    distanceToGround = hitInfo.distance;
                }
                if (Physics.Raycast(targetPosition, Vector3.up, out hitInfo, distanceToCeiling, LayerIndex.world.mask))
                {
                    distanceToCeiling = hitInfo.distance;
                }

                targetPosition += Vector3.up * Mathf.Max(0, Mathf.Min(distanceAboveGround - distanceToGround, distanceToCeiling - clearance));

                enemies.Add(new EnemyInfo
                {
                    healthComponent = enemy.healthComponent,
                    targetPosition = targetPosition,
                });
            }
        }

        private void UpdateCurrentEnemyPositions()
        {
            foreach (var enemy in enemies)
            {
                enemy.targetPosition = enemy.healthComponent.transform.position;
            }
        }

        private class EnemyInfo
        {
            public HealthComponent healthComponent;
            public Vector3 targetPosition;
        }
    }
}
