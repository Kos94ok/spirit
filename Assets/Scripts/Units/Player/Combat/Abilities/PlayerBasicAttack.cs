using Misc;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class PlayerBasicAttack : PlayerAbility {
		public override bool IsReady() {
			return true;
		}

		public override void OnCast(Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			Debug.Log("Casting");
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumRange() {
			return 7f;
		}
	}
}