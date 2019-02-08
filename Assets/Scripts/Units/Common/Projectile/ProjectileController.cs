using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace Units.Common.Projectile {
	public class ProjectileController {
		private GameObject ProjectileObject;
		private Vector3 Direction;
		private float MaximumSpeed;
		private float Acceleration;
		private readonly Timer LifeTime = new Timer();
		private float ProjectileRadius;
		private UnitAlliance Alliance;
		private int TargetRelationship;
		private Action<ProjectileAgent.EnemyHitCallbackPayload> EnemyHitCallback;
		private Action<ProjectileAgent.TimedOutCallbackPayload> TimedOutCallback;
		private Action<ProjectileAgent.ObstacleHitCallbackPayload> ObstacleHitCallback;

		private float Speed;
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
				float maximumSpeed,
				float acceleration,
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
			MaximumSpeed = maximumSpeed;
			Acceleration = acceleration;
			ProjectileRadius = projectileRadius;
			Alliance = alliance;
			TargetRelationship = targetRelationship;
			EnemyHitCallback = enemyHitCallback;
			TimedOutCallback = timedOutCallback;
			ObstacleHitCallback = obstacleHitCallback;
			
			LifeTime.Start(lifeTime);
			LifeTime.SetOnDoneAction(OnProjectileTimedOut);

			if (acceleration <= 0) {
				Speed = MaximumSpeed;
			}
			HitsRemaining = hitsAllowed;
			RaycastHits = new RaycastHit[HitsRemaining];
		}

		public void Update() {
			if (LifeTime.IsDone()) {
				return;
			}
			
			var deltaTime = Time.deltaTime;
			var position = ProjectileObject.transform.position;
			//var origin = position - ProjectileRadius * Direction;
			var origin = position;

			var targetHits = new List<TargetHit>();
			
			var hitCount = Physics.SphereCastNonAlloc(origin, ProjectileRadius, Direction, RaycastHits, MaximumSpeed * deltaTime, Layers.Walkable | Layers.LevelGeometry);
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
			
			hitCount = Physics.SphereCastNonAlloc(origin, ProjectileRadius, Direction, RaycastHits, MaximumSpeed * deltaTime, Layers.PlayerHitbox | Layers.NpcHitbox);
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
					OnEnemyHit(targetHit.Target, unitStats, targetHit.Hit.point);
				} else {
					OnObstacleHit(targetHit.Hit.point);
				}
				ProjectileObject.transform.position = targetHit.Hit.point;

				if (HitsRemaining == 0) {
					break;
				}
			}

			Speed += Acceleration * deltaTime;
			Speed = Mathf.Min(Speed, MaximumSpeed);
			ProjectileObject.transform.position = position + Speed * Direction * deltaTime;
		}
		
		private void OnEnemyHit(GameObject target, UnitStats unitStats, Vector3 collisionPoint) {
			HitsRemaining -= 1;
			var payload = new ProjectileAgent.EnemyHitCallbackPayload {
				Enemy = target,
				EnemyStats = unitStats,
				CollisionPoint = collisionPoint,
				Projectile = this
			};
			EnemyHitCallback(payload);
			if (!IsAlive()) {
				LifeTime.Stop();
			}
		}

		private void OnProjectileTimedOut() {
			HitsRemaining = 0;
			var payload = new ProjectileAgent.TimedOutCallbackPayload {
				Projectile = this
			};
			TimedOutCallback(payload);
		}

		private void OnObstacleHit(Vector3 collisionPoint) {
			HitsRemaining -= 1;
			var payload = new ProjectileAgent.ObstacleHitCallbackPayload {
				Projectile = this,
				CollisionPoint = collisionPoint
			};
			ObstacleHitCallback(payload);
			if (!IsAlive()) {
				LifeTime.Stop();
			}
		}

		public GameObject GetGameObject() {
			return ProjectileObject;
		}

		public Vector3 GetDirection() {
			return Direction;
		}

		public bool IsAlive() {
			return LifeTime.IsRunning() && HitsRemaining > 0;
		}
	}
}