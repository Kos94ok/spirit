using Misc;
using UnityEngine;

namespace Units.Player.Targeting {
	public class TargetedEnemy {
		private readonly GameObject Target;
		private readonly TargetedEnemyEffect Effect;

		public TargetedEnemy(GameObject enemy) {
			Target = enemy;
			Effect = Target.AddComponent<TargetedEnemyEffect>();
		}

		public Vector3 GetPosition() {
			return Target.transform.position;
		}
		public bool IsSame(GameObject target) {
			return target.Equals(Target);
		}
		public void RemoveEffect() {
			Object.Destroy(Effect);
		}
	}
}