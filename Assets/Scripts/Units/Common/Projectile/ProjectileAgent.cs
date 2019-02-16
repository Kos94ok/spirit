using System;
using System.Collections;
using System.Collections.Generic;
using Misc;
using Misc.ObjectPool;
using Units.Player.Movement;
using UnityEngine;

namespace Units.Common.Projectile {
	public class ProjectileAgent : MonoBehaviour {

		public class Builder {
			private readonly Vector3 Position;
			private Maybe<Vector3> TargetPoint = Maybe<Vector3>.None;
			private Maybe<GameObject> TargetUnit = Maybe<GameObject>.None;
			private Maybe<Vector3> TargetDirection = Maybe<Vector3>.None;
			private Maybe<Func<Vector3>> GetTargetDirectionFunc = Maybe<Func<Vector3>>.None;
			private float MaximumSpeed = 5f;
			private float Acceleration = Mathf.Infinity;
			private float LifeTime = 60f;
			private int HitsAllowed = 1;
			private float InitialDelay;
			private int ProjectileCount = 1;
			private float ProjectileDelay = 0.5f;
			private bool LeadTarget;
			private bool NormalizeHeight;
			private float ProjectileRadius = 0.1f;
			private Prefab ProjectileResource = Prefab.GenericProjectile;
			private UnitAlliance Alliance;
			private int TargetRelationship = UnitRelationship.Enemy;
			
			private Maybe<Action<EnemyHitCallbackPayload>> EnemyHitCallback = Maybe<Action<EnemyHitCallbackPayload>>.None;
			private Maybe<Action<TimedOutCallbackPayload>> TimedOutCallback = Maybe<Action<TimedOutCallbackPayload>>.None;
			private Maybe<Action<ObstacleHitCallbackPayload>> ObstacleHitCallback = Maybe<Action<ObstacleHitCallbackPayload>>.None;

			public Builder(Vector3 position, UnitAlliance alliance) {
				Position = position;
				Alliance = alliance;
			}

			public Builder SetTargetPoint(Vector3 targetPoint) {
				TargetPoint = Maybe<Vector3>.Some(targetPoint);
				return this;
			}
			
			public Builder SetTargetUnit(GameObject targetUnit) {
				TargetUnit = Maybe<GameObject>.Some(targetUnit);
				return this;
			}
			
			public Builder SetTargetDirection(Vector3 targetDirection) {
				TargetDirection = Maybe<Vector3>.Some(targetDirection);
				return this;
			}

			public Builder SetTargetDirection(Func<Vector3> getTargetDirection) {
				GetTargetDirectionFunc = Maybe<Func<Vector3>>.Some(getTargetDirection);
				return this;
			}

			public Builder SetMaximumSpeed(float maximumSpeed) {
				MaximumSpeed = maximumSpeed;
				return this;
			}

			public Builder SetAcceleration(float acceleration) {
				Acceleration = acceleration;
				return this;
			}

			public Builder SetLifeTime(float lifeTime) {
				LifeTime = lifeTime;
				return this;
			}

			public Builder SetHitsAllowed(int hitsAllowed) {
				HitsAllowed = hitsAllowed;
				return this;
			}

			public Builder SetInitialDelay(float initialDelay) {
				InitialDelay = initialDelay;
				return this;
			}
			
			public Builder SetProjectileCount(int projectileCount, float projectileDelay) {
				ProjectileCount = projectileCount;
				ProjectileDelay = projectileDelay;
				return this;
			}

			public Builder SetLeadTarget(bool leadTarget) {
				LeadTarget = leadTarget;
				return this;
			}

			public Builder SetNormalizeHeight(bool normalizeHeight) {
				NormalizeHeight = normalizeHeight;
				return this;
			}

			public Builder SetProjectileRadius(float projectileRadius) {
				ProjectileRadius = projectileRadius;
				return this;
			}
			
			public Builder SetProjectileResource(Prefab prefab) {
				ProjectileResource = prefab;
				return this;
			}

			public Builder SetTargetRelationship(int targetRelationship) {
				TargetRelationship = targetRelationship;
				return this;
			}
			
			public Builder SetEnemyHitCallback(Action<EnemyHitCallbackPayload> callback) {
				EnemyHitCallback = Maybe<Action<EnemyHitCallbackPayload>>.Some(callback);
				return this;
			}

			public Builder SetTimedOutCallback(Action<TimedOutCallbackPayload> callback) {
				TimedOutCallback = Maybe<Action<TimedOutCallbackPayload>>.Some(callback);
				return this;
			}

			public Builder SetObstacleHitCallback(Action<ObstacleHitCallbackPayload> callback) {
				ObstacleHitCallback = Maybe<Action<ObstacleHitCallbackPayload>>.Some(callback);
				return this;
			}

			public ProjectileAgent Create() {
				var projectileAgentContainer = new GameObject();
				var projectileAgent = projectileAgentContainer.AddComponent<ProjectileAgent>();
				projectileAgentContainer.name = "ProjectileAgent";

				var targetPoint = Vector3.zero;
				if (TargetUnit.HasValue && !LeadTarget) {
					targetPoint = TargetUnit.Value.transform.position;
				} else if (TargetUnit.HasValue && LeadTarget) {
					targetPoint = GetTargetInterceptionPoint();
				} else if (TargetPoint.HasValue) {
					targetPoint = TargetPoint.Value;
				}

				if (NormalizeHeight) {
					targetPoint.y = Position.y;
				}

				var targetDirection = TargetDirection.HasValue ? TargetDirection.Value : targetPoint - Position;
				var getTargetDirectionFunc = GetTargetDirectionFunc.HasValue ? GetTargetDirectionFunc.Value : () => targetDirection;
				
				projectileAgent.Init(Position, getTargetDirectionFunc, MaximumSpeed, Acceleration, LifeTime, HitsAllowed, InitialDelay, ProjectileCount, ProjectileDelay,
					ProjectileRadius, Alliance, TargetRelationship, ProjectileResource, EnemyHitCallback, TimedOutCallback, ObstacleHitCallback);
				return projectileAgent;
			}

			private Vector3 GetTargetInterceptionPoint() {
				var targetPosition = TargetUnit.Value.transform.position;
				var movementAgentExtensions = TargetUnit.Value.GetComponent<CharacterControllerExtension>();
				if (movementAgentExtensions == null) {
					return targetPosition;
				}
				var targetVelocity = movementAgentExtensions.GetCurrentVelocity();
				var interceptTime = Utility.FirstOrderInterceptTime(MaximumSpeed, TargetUnit.Value.transform.position - Position, targetVelocity);
				if (interceptTime > 0 && Mathf.Abs(Acceleration) > 0.01f) {
					interceptTime += MaximumSpeed / Acceleration / 2;
				}
				return targetPosition + targetVelocity * interceptTime;
			}
		}

		public struct EnemyHitCallbackPayload {
			public GameObject Enemy;
			public UnitStats EnemyStats;
			public Vector3 CollisionPoint;
			public ProjectileController Projectile;
		}

		public struct TimedOutCallbackPayload {
			public ProjectileController Projectile;
		}
		
		public struct ObstacleHitCallbackPayload {
			public Vector3 CollisionPoint;
			public ProjectileController Projectile;
		}

		private readonly ObjectPool ObjectPool = AutowireFactory.GetInstanceOf<ObjectPool>();

		private Vector3 StartingPosition;
		private Func<Vector3> GetDirection;
		private float MaximumSpeed;
		private float Acceleration;
		private float LifeTime;
		private float ProjectileRadius;
		private UnitAlliance Alliance;
		private int TargetRelationship;
		private Prefab ProjectilePrefab;
		
		private int HitsAllowed;
			
		private Maybe<Action<EnemyHitCallbackPayload>> EnemyHitCallback;
		private Maybe<Action<TimedOutCallbackPayload>> TimedOutCallback;
		private Maybe<Action<ObstacleHitCallbackPayload>> ObstacleHitCallback;

		private readonly List<ProjectileController> RegisteredProjectiles = new List<ProjectileController>();
		
		private void Init(
				Vector3 startingPosition,
				Func<Vector3> getDirection,
				float maximumSpeed,
				float acceleration,
				float lifeTime,
				int hitsAllowed,
				float initialDelay,
				int projectileCount,
				float projectileDelay,
				float projectileRadius,
				UnitAlliance alliance,
				int targetRelationship,
				Prefab projectilePrefab,
				Maybe<Action<EnemyHitCallbackPayload>> enemyHitCallback,
				Maybe<Action<TimedOutCallbackPayload>> timedOutCallback,
				Maybe<Action<ObstacleHitCallbackPayload>> obstacleHitCallback) {

			StartingPosition = startingPosition;
			GetDirection = getDirection;
			MaximumSpeed = maximumSpeed;
			Acceleration = acceleration;
			LifeTime = lifeTime;
			ProjectileRadius = projectileRadius;
			ProjectilePrefab = projectilePrefab;
			Alliance = alliance;
			TargetRelationship = targetRelationship;
			EnemyHitCallback = enemyHitCallback;
			TimedOutCallback = timedOutCallback;
			ObstacleHitCallback = obstacleHitCallback;

			HitsAllowed = hitsAllowed;

			for (var i = 0; i < projectileCount; i++) {
				StartCoroutine(SpawnProjectileAfterDelay(initialDelay + i * projectileDelay));
			}
			StartCoroutine(AgentSelfDestruct());
		}

		private void Update() {
			var activeProjectiles = RegisteredProjectiles.FindAll(projectile => projectile.IsAlive());
			foreach (var projectile in activeProjectiles) {
				projectile.Update();
			}
		}
		
		private void SpawnProjectile() {
			var projectile = ObjectPool.Obtain(ProjectilePrefab);
			projectile.SetActive(true);
			var projectileController = new ProjectileController();
			var direction = GetDirection().normalized;
			projectileController.Init(projectile, direction, MaximumSpeed, Acceleration, LifeTime,  HitsAllowed, ProjectileRadius,
				Alliance, TargetRelationship, OnProjectileEnemyHit, OnProjectileTimedOut, OnProjectileObstacleHit);
			projectile.transform.position = StartingPosition;
			projectile.transform.rotation = Quaternion.Euler(direction);
			RegisteredProjectiles.Add(projectileController);
		}

		private void OnProjectileEnemyHit(EnemyHitCallbackPayload payload) {
			if (!payload.Projectile.IsAlive()) {
				StartCoroutine(ProjectileSelfDestruct(payload.Projectile));
			}
			
			if (!EnemyHitCallback.HasValue) {
				return;
			}
			EnemyHitCallback.Value(payload);
		}

		private void OnProjectileTimedOut(TimedOutCallbackPayload payload) {
			StartCoroutine(ProjectileSelfDestruct(payload.Projectile));
			
			if (!TimedOutCallback.HasValue) {
				return;
			}
			TimedOutCallback.Value(payload);
		}

		private void OnProjectileObstacleHit(ObstacleHitCallbackPayload payload) {
			if (!payload.Projectile.IsAlive()) {
				StartCoroutine(ProjectileSelfDestruct(payload.Projectile));
			}
			
			if (!ObstacleHitCallback.HasValue) {
				return;
			}
			ObstacleHitCallback.Value(payload);
		}

		private IEnumerator SpawnProjectileAfterDelay(float delay) {
			yield return new WaitForSeconds(delay);
			SpawnProjectile();
		}

		private IEnumerator ProjectileSelfDestruct(ProjectileController projectile) {
			var particleSystems = projectile.GetGameObject().GetComponentsInChildren<ParticleSystem>();
			foreach (var system in particleSystems) {
				var emission = system.emission;
				emission.enabled = false;
			}
			
			yield return new WaitForSeconds(3);

			foreach (var system in particleSystems) {
				var emission = system.emission;
				emission.enabled = true;
			}
			ObjectPool.Return(ProjectilePrefab, projectile.GetGameObject());
			RegisteredProjectiles.Remove(projectile);
		}
		
		private IEnumerator AgentSelfDestruct() {
			yield return new WaitForSeconds(3);

			if (RegisteredProjectiles.Count > 0) {
				StartCoroutine(AgentSelfDestruct());
			} else {
				Destroy(transform.gameObject);
			}
		}
	}
}