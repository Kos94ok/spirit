using Misc;
using Units.Common.Lightning;
using Units.Common.Projectile;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class PlayerProjectileAttack : PlayerAbility {
		
		private const float Damage = 10;
		
		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);
			targetPosition.y = Utility.GetGroundPosition(targetPosition).y + 3f;

			var builder = new ProjectileAgent.Builder(sourcePosition, UnitAlliance.Player)
				.SetTargetDirection(targetPosition - sourcePosition)
				.SetMaximumSpeed(Vector3.Distance(targetPosition, sourcePosition))
				.SetHitsAllowed(2)
				.SetLifeTime(1)
				.SetTimedOutCallback(OnStageOneTimedOut);
			
			builder.Create();
			Cooldown.Start(0.7f);
		}

		private void OnEnemyHit(ProjectileAgent.EnemyHitCallbackPayload payload) {
			payload.EnemyStats.DealDamage(Damage);
		}

		private void OnStageOneTimedOut(ProjectileAgent.TimedOutCallbackPayload payload) {
			var builder = new ProjectileAgent.Builder(payload.Projectile.GetGameObject().transform.position, UnitAlliance.Player)
				.SetMaximumSpeed(10f)
				.SetProjectileResource(Prefab.AwakenedDwellerBasicAttackProjectile)
				.SetTargetDirection(() => Quaternion.Euler(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)) * Vector3.down)
				.SetProjectileCount(10, 0.00f)
				.SetEnemyHitCallback(OnEnemyHit);
			builder.Create();
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit | AbilityTargetType.Point;
		}

		public override float GetMaximumCastRange() {
			return float.PositiveInfinity;
		}
	}
}