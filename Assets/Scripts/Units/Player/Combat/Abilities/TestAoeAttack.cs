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
				targetPosition.y = sourcePosition.y;
			} else {
				targetPosition = targetPoint.Value;
				targetPosition.y = sourcePosition.y;
			}

			var sector = Random.Range(80f, 100);
			var lightningCount = Random.Range(3, 7);
			var targetVector = targetPosition - sourcePosition;

			for (var i = 0; i < lightningCount; i++) {
				var angleOffset = (sector / lightningCount) * i - sector / 2;
				var adjustedTargetVector = Quaternion.Euler(0, angleOffset, 0) * targetVector;
				new LightningAgent.Builder(sourcePosition, sourcePosition + adjustedTargetVector.normalized * Random.Range(1.7f, 2.3f))
					.SetAngularDeviation(40f)
					.SetSpeed(1000f)
					.SetBranchingChance(0.4f)
					.SetBranchingFactor(0.5f)
					.SetSmoothFactor(0.7f)
					.SetFragmentLifeTime(0.05f)
					.Create();
			}

			Cooldown.Start(0.05f);
		}
		
		public override int GetTargetType() {
			return AbilityTargetType.Point;
		}

		public override float GetMaximumCastRange() {
			return float.PositiveInfinity;
		}
	}
}