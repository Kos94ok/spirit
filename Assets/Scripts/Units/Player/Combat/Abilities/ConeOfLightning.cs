using Misc;
using Units.Common.Lightning;
using Units.Player.Combat.Abilities.Utilities;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class ConeOfLightning : PlayerAbility {

		private const float Damage = 2;
		private const float AutoTargetWidth = .4f;
		
		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;

			var targetPosition = targetUnit.HasValue ? targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition() : targetPoint.Value;
			targetPosition.y = sourcePosition.y;

			var sector = Random.Range(80f, 100);
			var lightningCount = Random.Range(3, 6);
			var targetVector = targetPosition - sourcePosition;

			for (var i = 0; i < lightningCount; i++) {
				var angleOffset = sector / lightningCount * i - sector / 2;
				var adjustedTargetVector = Quaternion.Euler(0, angleOffset, 0) * targetVector.normalized;
				
				Vector3 startingOffset;
				Vector3 newTargetPosition;
				Maybe<GameObject> newTargetUnit;
				AbilityUtils.RetargetLightningAbility(sourcePosition, sourcePosition + adjustedTargetVector * Random.Range(1.2f, 1.7f), targetUnit, AutoTargetWidth,
					out newTargetPosition, out newTargetUnit, out startingOffset);
				
				new LightningAgent.Builder(sourcePosition, newTargetPosition)
					.SetAngularDeviation(40f)
					.SetSpeed(1000f)
					.SetBranchingChance(0.4f)
					.SetBranchingFactor(0.5f)
					.SetSmoothFactor(0.7f)
					.SetFragmentLifeTime(0.10f)
					.SetTargetReachedCallback(OnLightningTargetReached, newTargetUnit)
					.Create();
			}

			Cooldown.Start(0.10f);
		}

		public override void OnTargetUnitReached(GameObject targetUnit, UnitStats targetStats) {
			targetStats.DealDamage(Damage);
		}
		
		public override int GetTargetType() {
			return AbilityTargetType.Point;
		}

		public override float GetMaximumCastRange() {
			return float.PositiveInfinity;
		}
	}
}