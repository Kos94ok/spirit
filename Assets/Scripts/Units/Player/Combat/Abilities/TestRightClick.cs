using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class TestRightClick : PlayerAbility {
		public class Data {
			public Maybe<GameObject> TargetUnit;
		}

		private const float Damage = 10;
		private const float AutoTargetWidth = .2f;

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = targetUnit.HasValue ? targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition() : targetPoint.Value;

			RaycastHit raycastHit;
			var ray = new Ray(sourcePosition, targetPosition - sourcePosition);
			if (Physics.SphereCast(ray, AutoTargetWidth, out raycastHit, GetMaximumCastRange(), Layers.EnemyHitbox)) {
				targetUnit = Maybe<GameObject>.Some(raycastHit.transform.parent.gameObject);
				targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			}
			
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