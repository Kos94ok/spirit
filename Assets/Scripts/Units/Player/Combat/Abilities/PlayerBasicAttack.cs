using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class PlayerBasicAttack : PlayerAbility {
		
		private const float Damage = 10;
		private const float AutoTargetWidth = .5f;
		
		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);

			Vector3 startingOffset;
			AbilityUtils.RetargetLightningAbility(sourcePosition, targetPosition, targetUnit, AutoTargetWidth, out targetPosition, out targetUnit, out startingOffset);

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(50f)
				.SetSpeed(1000f)
				.SetSmoothFactor(0.6f)
				.SetMaximumBranchDepth(3)
				.SetFragmentResource(Prefab.LightningEffectFragment)
				.SetTargetReachedCallback(OnLightningTargetReached, targetUnit);
			builder.Create();
			Cooldown.Start(0.7f);
		}
		
		public override void OnTargetUnitReached(GameObject targetUnit, UnitStats targetStats) {
			targetStats.DealDamage(Damage);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 3f;
		}
	}
}