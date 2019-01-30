using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class TestAoeAttack : PlayerAbility {

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;

			Vector3 targetPosition;
			if (targetUnit.HasValue) {
				targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			} else {
				targetPosition = targetPoint.Value;
				targetPosition.y = sourcePosition.y;
			}

			var sector = 90f;
			var lightningCount = 10;
			var targetVector = targetPosition - sourcePosition;

			for (var i = 0; i < lightningCount; i++) {
				var angleOffset = (sector / lightningCount) * i - sector / 2;
				var adjustedTargetVector = Quaternion.Euler(0, angleOffset, 0) * targetVector;
				new LightningAgent.Builder(sourcePosition, sourcePosition + adjustedTargetVector.normalized * 1.2f)
					.SetAngularDeviation(40f)
					.SetSpeed(1000f)
					.Create();
			}

			Cooldown.Start(0.20f);
		}
		
		public override int GetTargetType() {
			return AbilityTargetType.Point;
		}

		public override float GetMaximumCastRange() {
			return float.PositiveInfinity;
		}
	}
}