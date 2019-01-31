using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class TestUltraLightning : PlayerAbility {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			Vector3 targetPosition;
			if (targetUnit.HasValue) {
				targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			} else {
				targetPosition = targetPoint.Value;
				targetPosition.y = sourcePosition.y;
			}

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(120f)
				.SetSpeed(300f)
				.SetFragmentLifeTime(0.12f)
				.SetFragmentParticleLifeTime(2f)
				.SetBranchingChance(0.00f)
				.SetBranchingFactor(0.5f)
				.SetSmoothFactor(0.65f)
				.SetMaximumBranchDepth(3)
				.SetFragmentResource(Prefab.LongLightningEffectFragment);
			builder.Create();
			Cooldown.Start(0.3f);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 10f;
		}
	}
}