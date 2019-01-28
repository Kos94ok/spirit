using Misc;
using UnityEngine;

namespace Units.Player.Combat {
	public abstract class PlayerAbility {
		public abstract bool IsReady();
		public abstract void OnCast(Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit);
		public abstract int GetTargetType();
		public abstract float GetMaximumRange();

		public bool IsTargetingSelf() {
			return (GetTargetType() & AbilityTargetType.Self) > 0;
		}
		
		public bool IsTargetingUnit() {
			return (GetTargetType() & AbilityTargetType.Unit) > 0;
		}
		
		public bool IsTargetingPoint() {
			return (GetTargetType() & AbilityTargetType.Point) > 0;
		}
	}
}