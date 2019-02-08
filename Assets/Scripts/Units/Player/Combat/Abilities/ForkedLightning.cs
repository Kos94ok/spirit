using System.Numerics;
using Misc;
using Units.Common.Lightning;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Units.Player.Combat.Abilities {
	public class ForkedLightning : PlayerAbility {

		private const float Damage = 10;
		private const float AutoTargetWidth = .5f;

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);

			AbilityUtils.RetargetLightningAbility(sourcePosition, targetPosition, targetUnit, AutoTargetWidth, out targetPosition, out targetUnit, out var startingOffset);

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition, targetUnit)
				.SetAngularDeviation(90f)
				.SetFragmentLifeTime(0.05f)
				.SetBranchingChance(0.00f)
				.SetBranchingFactor(0.5f)
				.SetSmoothFactor(0.8f)
				.SetMaximumBranchDepth(3)
				.SetStartingOffset(startingOffset)
				.SetTargetUnitReachedCallback(OnTargetUnitReached);
			builder.Create();
			builder.Create();
			builder.Create();
			Cooldown.Start(0.5f);
		}
		
		private void OnTargetUnitReached(LightningAgent.TargetUnitReachedCallbackPayload payload) {
			payload.TargetStats.DealDamage(Damage);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Point | AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 6f;
		}
	}
}