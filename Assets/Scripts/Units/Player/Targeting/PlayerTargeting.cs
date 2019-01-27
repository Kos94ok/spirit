using Misc;
using UI.UserInput;
using UnityEngine;

namespace Units.Player.Targeting {
	public class PlayerTargeting : MonoBehaviour {
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();

		private Maybe<TargetedEnemy> TargetedEnemy;
		private void Update() {
			var hoveredEnemy = MouseStatus.GetHoveredEnemy();
			if (!hoveredEnemy.HasValue) {
				ResetTarget();
				return;
			}

			SetTarget(hoveredEnemy.Value);
		}

		private void SetTarget(GameObject target) {
			if (TargetedEnemy.HasValue && TargetedEnemy.Value.IsSame(target)) {
				return;
			}
			
			ResetTarget();
			TargetedEnemy = Maybe<TargetedEnemy>.Some(new TargetedEnemy(target));
		}

		private void ResetTarget() {
			if (!TargetedEnemy.HasValue) {
				return;
			}

			TargetedEnemy.Value.RemoveEffect();
			TargetedEnemy = Maybe<TargetedEnemy>.None;
		}

		public Maybe<TargetedEnemy> GetTargetedEnemy() {
			return TargetedEnemy;
		}
	}
}