using Misc;
using Settings;
using UI.UserInput;
using UnityEngine;

namespace Units.Player.Targeting {
	public class PlayerTargeting : MonoBehaviour {
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		private Maybe<GameObject> LockedTarget;
		private Maybe<GameObject> HoveredTarget;
		private Maybe<TargetedEnemy> TargetedEnemy;

		private void Update() {
			var hoveredEnemy = MouseStatus.GetHoveredEnemy();

			if (hoveredEnemy.HasValue && CommandStatus.IsAllInactive(CommandBinding.Command.MoveToMouse, CommandBinding.Command.ForceMoveToMouse)) {
				HoveredTarget = Maybe<GameObject>.Some(hoveredEnemy.Value);
			} else {
				HoveredTarget = Maybe<GameObject>.None;
			}
			
			if (hoveredEnemy.HasValue && CommandStatus.IsIssuedThisFrame(CommandBinding.Command.MoveToMouse)) {
				LockedTarget = Maybe<GameObject>.Some(hoveredEnemy.Value);
			} else if (CommandStatus.IsInactive(CommandBinding.Command.MoveToMouse)) {
				LockedTarget = Maybe<GameObject>.None;
			}
			
			UpdateTargetedEnemy();
		}

		private void UpdateTargetedEnemy() {
			var newTarget = Maybe<GameObject>.None;
			if (LockedTarget.HasValue) {
				newTarget = LockedTarget;
			}

			if (HoveredTarget.HasValue) {
				newTarget = HoveredTarget;
			}

			if (newTarget.HasValue == TargetedEnemy.HasValue && (!TargetedEnemy.HasValue || TargetedEnemy.Value.IsSame(newTarget.Value))) {
				return;
			}
			
			if (newTarget.HasValue && !TargetedEnemy.HasValue) {
				TargetedEnemy = Maybe<TargetedEnemy>.Some(new TargetedEnemy(newTarget.Value));
				TargetedEnemy.Value.CreateEffect();
			} else {
				TargetedEnemy.Value.RemoveEffect();
				TargetedEnemy = Maybe<TargetedEnemy>.None;
			}
		}

		public Maybe<GameObject> GetTargetedEnemy() {
			return TargetedEnemy.HasValue ? Maybe<GameObject>.Some(TargetedEnemy.Value.GetTarget()) : Maybe<GameObject>.None;
		}
	}
}