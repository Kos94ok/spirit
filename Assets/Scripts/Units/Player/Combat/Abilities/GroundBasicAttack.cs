using Misc;
using UnityEngine;

namespace Units.Player.Combat.Abilities {
	public class GroundBasicAttack : PlayerAbility {
		public override bool IsReady() {
			return true;
		}

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			Debug.Log("Casting Q");
		}

		public override void Update() {
		}
		
		public override int GetTargetType() {
			return AbilityTargetType.Point;
		}

		public override float GetMaximumRange() {
			return 3f;
		}
	}
}