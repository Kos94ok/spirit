using Units.Player.Targeting;
using UnityEngine;

namespace Units.Player.Combat {
	public class PlayerCombat : MonoBehaviour {
		// ReSharper disable once Unity.RedundantEventFunction
		private void Start() {}

		public float GetBasicAttackRange() {
			return 10f;
		}
		
		public bool IsInRangeForBasicAttack(Vector3 target) {
			return Vector3.Distance(transform.position, target) <= 10f;
		}

		public bool IsInRangeForBasicAttack(TargetedEnemy target) {
			return IsInRangeForBasicAttack(target.GetPosition());
		}
	}
}