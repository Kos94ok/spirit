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
				.SetAngularDeviation(70f)
				.SetSpeed(1000f)
				.SetBranchingChance(0.20f)
				.SetBranchingFactor(3.50f)
				.SetMaximumBranchDepth(4)
				.SetFragmentResource(Resource.LongLightningEffectFragment)
				.SetFragmentParticleLifeTime(2f);
			builder.Create();
			Cooldown.Start(1f);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 10f;
		}
	}
}