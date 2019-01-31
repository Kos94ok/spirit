using System;
using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities.PlayerBasicAttack {
	public class PlayerBasicAttack : PlayerAbility {
		public class Data {
			public Maybe<GameObject> TargetUnit;
		}
		
		private const float Damage = 25;
		
		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			Vector3 targetPosition;
			if (targetUnit.HasValue) {
				targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			} else {
				targetPosition = targetPoint.Value;
				targetPosition.y = sourcePosition.y;
			}

			var callbackPayload = new Data {
				TargetUnit = targetUnit
			};

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(50f)
				.SetSpeed(1000f)
				.SetSmoothFactor(0.6f)
				.SetMaximumBranchDepth(3)
				.SetFragmentResource(Prefab.LightningEffectFragment)
				.SetTargetReachedCallback(OnTargetReached, callbackPayload);
			builder.Create();
			Cooldown.Start(0.7f);
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
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumCastRange() {
			return 10f;
		}
	}
}