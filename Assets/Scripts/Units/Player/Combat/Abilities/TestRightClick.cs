using System.Numerics;
using Misc;
using Units.Common.Lightning;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Units.Player.Combat.Abilities {
	public class TestRightClick : PlayerAbility {
		public class Data {
			public Maybe<GameObject> TargetUnit;
		}

		private const float Damage = 10;
		private const float AutoTargetWidth = .5f;

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = AbilityUtils.GetTargetPosition(sourcePosition, targetPoint, targetUnit);

			Vector3 startingOffset;
			AbilityUtils.RetargetLightningAbility(sourcePosition, targetPosition, targetUnit, AutoTargetWidth, out targetPosition, out targetUnit, out startingOffset);
			
			var callbackPayload = new Data {
				TargetUnit = targetUnit
			};

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(90f)
				.SetFragmentLifeTime(0.05f)
				.SetBranchingChance(0.00f)
				.SetBranchingFactor(0.5f)
				.SetSmoothFactor(0.8f)
				.SetMaximumBranchDepth(3)
				.SetStartingOffset(startingOffset)
				.SetTargetReachedCallback(OnTargetReached, callbackPayload);
			builder.Create();
			builder.Create();
			builder.Create();
			Cooldown.Start(0.5f);
		}
		
		private void OnTargetReached(object rawPayload) {
			var payload = (Data) rawPayload;
			if (!payload.TargetUnit.HasValue || payload.TargetUnit.Value == null) {
				return;
			}

			var targetUnit = payload.TargetUnit.Value;
			var stats = targetUnit.GetComponent<UnitStats>();
			if (stats == null) {
				return;
			}

			stats.DealDamage(Damage);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Point | AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 6f;
		}
	}
}