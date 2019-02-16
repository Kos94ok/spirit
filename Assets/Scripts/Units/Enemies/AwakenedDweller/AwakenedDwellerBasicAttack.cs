using Misc;
using Misc.ObjectPool;
using Units.Common.Projectile;
using Units.Player.Combat;
using Units.Player.Combat.Abilities;
using UnityEngine;

namespace Units.Enemies.AwakenedDweller {
	public class AwakenedDwellerBasicAttack : EnemyAbility {
		private readonly ObjectPool ObjectPool = AutowireFactory.GetInstanceOf<ObjectPool>();

		private const float Damage = 4f;
		private const float MaxRange = 8f;
		private const float ProjectileRadius = 0.1f;
		private const Prefab ProjectilePrefab = Prefab.AwakenedDwellerBasicAttackProjectile;
		private const Prefab ChargeEffectPrefab = Prefab.AwakenedDwellerBasicAttackChargeEffect;
		private const Prefab ProjectileHitEffectPrefab = Prefab.AwakenedDwellerBasicAttackProjectileHitEffect;

		private readonly Timer ChargeTimer = new Timer();
		
		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = AbilityUtils.GetCasterPosition(caster);
			var targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);

			var projectileSpawnPoint = GetProjectileSpawnPoint(sourcePosition, targetPosition);

			var chargeEffect = ObjectPool.ObtainForDuration(ChargeEffectPrefab, 1.1f);
			chargeEffect.transform.position = projectileSpawnPoint;
			
			ChargeTimer.Start(1.0f);
			ChargeTimer.SetOnDoneAction(() => {
				targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);
				var builder = new ProjectileAgent.Builder(projectileSpawnPoint, UnitAlliance.Corruption)
					.SetMaximumSpeed(5f)
					.SetAcceleration(10f)
					.SetLifeTime(5f)
					.SetTargetUnit(targetUnit.Value)
					.SetLeadTarget(true)
					.SetProjectileRadius(ProjectileRadius)
					.SetEnemyHitCallback(OnEnemyHit)
					.SetObstacleHitCallback(OnObstacleHit)
					.SetProjectileResource(ProjectilePrefab);
				builder.Create();
			});
			
			Cooldown.Start(2.5f);
		}

		private void OnEnemyHit(ProjectileAgent.EnemyHitCallbackPayload payload) {
			payload.EnemyStats.DealDamage(Damage);

			var projectilePosition = payload.Projectile.GetGameObject().transform.position;
			var hitEffect = ObjectPool.ObtainForDuration(ProjectileHitEffectPrefab, 3f);
			hitEffect.transform.position = projectilePosition;
			hitEffect.transform.rotation = Quaternion.LookRotation(payload.Projectile.GetDirection());
		}

		private void OnObstacleHit(ProjectileAgent.ObstacleHitCallbackPayload payload) {
			var projectilePosition = payload.Projectile.GetGameObject().transform.position;
			var hitEffect = ObjectPool.ObtainForDuration(ProjectileHitEffectPrefab, 3f);
			hitEffect.transform.position = projectilePosition;
			hitEffect.transform.rotation = Quaternion.LookRotation(payload.Projectile.GetDirection());
		}

		public static Vector3 GetProjectileSpawnPoint(Vector3 casterPosition, Vector3 targetPosition) {
			return casterPosition + (targetPosition - casterPosition).normalized * 0.2f;
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit | AbilityTargetType.Point;
		}

		public override float GetMaximumCastRange() {
			return MaxRange;
		}
	}
}