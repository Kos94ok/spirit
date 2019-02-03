using Misc;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public static class AbilityUtils {
		public static Vector3 GetTargetPosition(Vector3 sourcePosition, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			Vector3 targetPosition;
			if (targetUnit.HasValue) {
				targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			} else {
				targetPosition = targetPoint.Value;
				targetPosition.y = sourcePosition.y;
			}
			return targetPosition;
		}

		public static void RetargetLightningAbility(Vector3 sourcePosition, Vector3 targetPosition, Maybe<GameObject> targetUnit, float lightningRadius,
				out Vector3 newTargetPosition, out Maybe<GameObject> newTargetUnit, out Vector3 startingOffset) {
			RaycastHit raycastHit;
			var ray = new Ray(sourcePosition, targetPosition - sourcePosition);
			if (Physics.SphereCast(ray, lightningRadius, out raycastHit, Vector3.Distance(sourcePosition, targetPosition), Layers.EnemyHitbox)) {
				var angle = Mathf.Clamp(Vector3.SignedAngle(targetPosition - sourcePosition, raycastHit.point - sourcePosition, Vector3.up) * 8f, -30f, 30f);
				startingOffset = new Vector3(angle, 0, angle);
				newTargetUnit = Maybe<GameObject>.Some(raycastHit.transform.parent.gameObject);
				newTargetPosition = newTargetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();
			} else {
				startingOffset = Vector3.zero;
				newTargetPosition = targetPosition;
				newTargetUnit = targetUnit;
			}
		}
	}
}