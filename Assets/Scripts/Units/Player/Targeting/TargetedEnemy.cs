using Misc;
using UnityEngine;

namespace Units.Player.Targeting {
	public class TargetedEnemy {
		private readonly GameObject Target;
		private TargetedEnemyEffect Effect;

		public TargetedEnemy(GameObject enemy) {
			Target = enemy;
		}

		public GameObject GetTarget() {
			return Target;
		}
		public Vector3 GetPosition() {
			return Target.transform.position;
		}
		public bool IsSame(GameObject target) {
			return target.Equals(Target);
		}

		public void CreateEffect() {
			Effect = Target.AddComponent<TargetedEnemyEffect>();
		}
		public void RemoveEffect() {
			if (Effect == null) {
				return;
			}
			Object.Destroy(Effect);
		}
	}
}