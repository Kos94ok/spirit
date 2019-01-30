using Misc;
using Settings;
using UI.UserInput;
using UnityEngine;

namespace Units.Player.Combat {
	public abstract class PlayerAbility {
		protected Timer Cooldown = new Timer();
		protected readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();
		public abstract void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit);
		public abstract int GetTargetType();
		public abstract float GetMaximumCastRange();
		public virtual void Update() {
			Cooldown.Tick();
		}
		public bool IsReady() {
			return Cooldown.IsDone();
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