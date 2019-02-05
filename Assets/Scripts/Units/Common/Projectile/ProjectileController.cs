using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace Units.Common.Projectile {
	public class ProjectileController {
		private GameObject ProjectileObject;
		private Vector3 Direction;
		private float Speed;
		private readonly Timer LifeTime = new Timer();
		private float ProjectileRadius;
		private UnitAlliance Alliance;
		private int TargetRelationship;
		private Action<ProjectileAgent.EnemyHitCallbackPayload> EnemyHitCallback;
		private Action<ProjectileAgent.TimedOutCallbackPayload> TimedOutCallback;
		private Action<ProjectileAgent.ObstacleHitCallbackPayload> ObstacleHitCallback;
		
		private int HitsRemaining;
		private readonly List<GameObject> PreviousTargets = new List<GameObject>();
		
		private RaycastHit[] RaycastHits;

		private struct TargetHit {
			public RaycastHit Hit;
			public GameObject Target;
		}

		public void Init(
				GameObject projectileObject,
				Vector3 direction,
				float speed,
				float lifeTime,
				int hitsAllowed,
				float projectileRadius,
				UnitAlliance alliance,
				int targetRelationship,
				Action<ProjectileAgent.EnemyHitCallbackPayload> enemyHitCallback,
				Action<ProjectileAgent.TimedOutCallbackPayload> timedOutCallback,
				Action<ProjectileAgent.ObstacleHitCallbackPayload> obstacleHitCallback) {
			ProjectileObject = projectileObject;
			Direction = direction;
			Speed = speed;
			LifeTime.Start(lifeTime);
			ProjectileRadius = projectileRadius;
			Alliance = alliance;
			TargetRelationship = targetRelationship;
			EnemyHitCallback = enemyHitCallback;
			TimedOutCallback = timedOutCallback;
			ObstacleHitCallback = obstacleHitCallback;

			HitsRemaining = hitsAllowed;
			RaycastHits = new RaycastHit[HitsRemaining];
		}

		public void Update() {
			var deltaTime = Time.deltaTime;
			var position = ProjectileObject.transform.position;
			var origin = position - ProjectileRadius * Direction;

			var targetHits = new List<TargetHit>();
			
			var hitCount = Physics.SphereCastNonAlloc(origin, ProjectileRadius, Direction, RaycastHits, Speed * deltaTime, Layers.Walkable | Layers.LevelGeometry);
			for (var i = 0; i < hitCount; i++) {
				var hitObstacle = RaycastHits[i].transform.gameObject;
				if (PreviousTargets.Contains(hitObstacle)) {
					continue;
				}
				
				PreviousTargets.Add(hitObstacle);
				targetHits.Add(new TargetHit {
					Hit = RaycastHits[i],
					Target = hitObstacle
				});
			}
			
			hitCount = Physics.SphereCastNonAlloc(origin, ProjectileRadius, Direction, RaycastHits, Speed * deltaTime, Layers.Hitbox);
			for (var i = 0; i < hitCount; i++) {
				var hitUnit = RaycastHits[i].transform.parent.gameObject;
				var unitStats = hitUnit.GetComponent<UnitStats>();
				if (unitStats == null || PreviousTargets.Contains(hitUnit) || !unitStats.IsInRelationship(Alliance, TargetRelationship)) {
					continue;
				}

				PreviousTargets.Add(hitUnit);
				targetHits.Add(new TargetHit {
					Hit = RaycastHits[i],
					Target = hitUnit
				});
			}
			
			targetHits.Sort((first, second) => {
				var firstDistance = Vector3.Distance(first.Target.transform.position, position);
				var secondDistance = Vector3.Distance(second.Target.transform.position, position); 
				return firstDistance.CompareTo(secondDistance);
			});
			
			foreach (var targetHit in targetHits) {
				var unitStats = targetHit.Target.GetComponent<UnitStats>();
				if (unitStats != null) {
					OnEnemyHit(targetHit.Target, unitStats);
				} else {
					OnObstacleHit();
				}
				ProjectileObject.transform.position = targetHit.Hit.point;

				if (HitsRemaining == 0) {
					break;
				}
			}

			LifeTime.Tick();
			if (LifeTime.IsDone() && HitsRemaining > 0) {
				OnProjectileTimedOut();
				return;
			}
			
			ProjectileObject.transform.position = position + Speed * Direction * deltaTime;
		}
		
		private void OnEnemyHit(GameObject target, UnitStats unitStats) {
			HitsRemaining -= 1;
			var payload = new ProjectileAgent.EnemyHitCallbackPayload {
				Enemy = target,
				EnemyStats = unitStats,
				Projectile = this
			};
			EnemyHitCallback(payload);
		}

		private void OnProjectileTimedOut() {
			HitsRemaining = 0;
			var payload = new ProjectileAgent.TimedOutCallbackPayload {
				Projectile = this
			};
			TimedOutCallback(payload);
		}

		private void OnObstacleHit() {
			HitsRemaining -= 1;
			var payload = new ProjectileAgent.ObstacleHitCallbackPayload {
				Projectile = this
			};
			ObstacleHitCallback(payload);
		}

		public GameObject GetGameObject() {
			return ProjectileObject;
		}

		public bool IsAlive() {
			return LifeTime.IsRunning() && HitsRemaining > 0;
		}
	}
}