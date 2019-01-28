using Misc;
using UnityEngine;

namespace Units.Player.Combat {
	public class QueuedPlayerAbility {
		private readonly PlayerAbility Ability;
		private readonly Maybe<Vector3> TargetPoint;
		private readonly Maybe<GameObject> TargetUnit;
		private readonly PlayerCombat.AbilityQueueReason Reason;

		public QueuedPlayerAbility(PlayerAbility ability, Vector3 target, PlayerCombat.AbilityQueueReason reason) {
			Ability = ability;
			Reason = reason;
			TargetPoint = Maybe<Vector3>.Some(target);
		}
		
		public QueuedPlayerAbility(PlayerAbility ability, GameObject target, PlayerCombat.AbilityQueueReason reason) {
			Ability = ability;
			Reason = reason;
			TargetUnit = Maybe<GameObject>.Some(target);
		}

		public PlayerAbility GetAbility() {
			return Ability;
		}

		public Maybe<Vector3> GetTargetPoint() {
			return TargetPoint;
		}

		public Maybe<GameObject> GetTargetUnit() {
			return TargetUnit;
		}

		public Vector3 GetTargetPosition() {
			return TargetUnit.HasValue ? TargetUnit.Value.transform.position : TargetPoint.Value;
		}

		public PlayerCombat.AbilityQueueReason GetReason() {
			return Reason;
		}
	}
}