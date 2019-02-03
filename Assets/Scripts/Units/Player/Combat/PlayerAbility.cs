using Misc;
using Settings;
using UI.UserInput;
using Units.Player.Combat.Abilities.Utilities;
using UnityEngine;

namespace Units.Player.Combat {
	public abstract class PlayerAbility {
		protected readonly Timer Cooldown = new Timer();
		protected readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();
		public abstract void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit);
		public virtual void OnTargetUnitReached(GameObject target, UnitStats targetStats) { }
		public abstract int GetTargetType();
		public abstract float GetMaximumCastRange();
		public virtual void Update() {
			Cooldown.Tick();
		}
		public bool IsReady() {
			return Cooldown.IsDone();
		}

		protected void OnLightningTargetReached(object rawPayload) {
			var payload = (BasicLightningCallbackData) rawPayload;
			if (!payload.TargetUnit.HasValue || payload.TargetUnit.Value == null) {
				return;
			}

			var targetUnit = payload.TargetUnit.Value;
			var stats = targetUnit.GetComponent<UnitStats>();
			if (stats == null) {
				return;
			}

			OnTargetUnitReached(targetUnit, stats);
		}

		public bool IsTargetingSelf() {
			return (GetTargetType() & AbilityTargetType.Self) > 0;
		}
		
		public bool IsTargetingUnit() {
			return (GetTargetType() & AbilityTargetType.Unit) > 0;
		}
		
		public bool IsTargetingPoint() {
			return (GetTargetType() & AbilityTargetType.Point) > 0 
				|| (GetTargetType() & AbilityTargetType.Unit) > 0 && CommandStatus.IsActive(CommandBinding.Command.ForceCast);
		}
	}
}