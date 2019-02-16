using Misc;
using Misc.TimerPool;
using Settings;
using UI.UserInput;
using Units.Player.Combat;
using UnityEngine;

namespace Units.Common {
	public abstract class UnitAbility {
		protected readonly Timer Cooldown = new Timer();
		public abstract void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit);
		public virtual void OnCancel() { }

		public void OnNotEnoughMana() {
			Cooldown.Start(0.5f);
		}
		public abstract int GetTargetType();
		public abstract float GetMaximumCastRange();

		public virtual float GetManaCost() {
			return 0;
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
		
		public virtual bool IsTargetingPoint() {
			return (GetTargetType() & AbilityTargetType.Point) > 0;
		}
	}
}