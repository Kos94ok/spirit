using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class TestRightClick : PlayerAbility {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = targetUnit.HasValue ? targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition() : targetPoint.Value;

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(50f)
				.SetBranchingChance(0.2f)
				.SetBranchingFactor(0.5f)
				.SetMaximumBranchDepth(3);
			builder.Create();
			builder.Create();
			builder.Create();
			Cooldown.Start(0.15f);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Point | AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 6f;
		}
	}
}