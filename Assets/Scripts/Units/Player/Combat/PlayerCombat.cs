using System.Collections.Generic;
using Misc;
using Settings;
using UI.ChatLog;
using UI.UserInput;
using Units.Common;
using Units.Player.Combat.Abilities;
using Units.Player.Targeting;
using UnityEngine;

namespace Units.Player.Combat {
	public class PlayerCombat : MonoBehaviour {
		public enum AbilityQueueReason {
			Cast,
			Range,
		}
		
		private PlayerTargeting Targeting;
		private readonly ChatLog ChatLog = AutowireFactory.GetInstanceOf<ChatLog>();
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		private Maybe<QueuedPlayerAbility> QueuedAbility = Maybe<QueuedPlayerAbility>.None;
		private readonly Dictionary<CommandBinding.Command, PlayerAbility> AbilityLibrary = new Dictionary<CommandBinding.Command, PlayerAbility>();
		
		private void Start() {
			Targeting = GetComponent<PlayerTargeting>();
			AbilityLibrary.Add(CommandBinding.Command.MoveToMouse, new PlayerBasicAttack());
			AbilityLibrary.Add(CommandBinding.Command.AbilityRightClick, new PlayerProjectileAttack());
			//AbilityLibrary.Add(CommandBinding.Command.AbilityRightClick, new ForkedLightning());
			AbilityLibrary.Add(CommandBinding.Command.AbilityQ, new ConeOfLightning());
			AbilityLibrary.Add(CommandBinding.Command.AbilityW, new TestUltraLightning());
			AbilityLibrary.Add(CommandBinding.Command.AbilityE, new TestPinkLightning());
		}

		private void Update() {
			var targetUnit = Targeting.GetTargetedEnemy();
			var targetPoint = MouseStatus.GetWalkableWorldPoint();
			var abilityToQueue = Maybe<QueuedPlayerAbility>.None;
			foreach (var entry in AbilityLibrary) {
				var command = entry.Key;
				var ability = entry.Value;
				ability.Update();
				if (!CommandStatus.IsActive(command) || !ability.IsReady()) {
					continue;
				}
				
				if (!IsInRangeToCast(ability) && (ability.IsTargetingPoint() && targetPoint.HasValue || ability.IsTargetingUnit() && targetUnit.HasValue)) {
					abilityToQueue = CreateQueuedAbility(ability, AbilityQueueReason.Range);
					continue;
				}

				if (CommandStatus.IsIssuedThisFrame(command) && command != CommandBinding.Command.MoveToMouse && ability.IsTargetingUnit() && !ability.IsTargetingPoint() && !targetUnit.HasValue) {
					ChatLog.Post("error_noTarget");
					continue;
				}

				if (ability.IsTargetingSelf()
						|| ability.IsTargetingUnit() && targetUnit.HasValue
						|| ability.IsTargetingPoint() && targetPoint.HasValue) {
					var adjustedTargetPoint = GetClosestTargetPointInRange(targetUnit, targetPoint, ability.GetMaximumCastRange());
					var adjustedTargetUnit = GetTargetUnitIfInRange(targetUnit, ability.GetMaximumCastRange());
					ability.OnCast(transform.gameObject, adjustedTargetPoint, adjustedTargetUnit);
					ClearQueuedAbility();
				}
			}

			if (CommandStatus.IsAnyIssuedThisFrame(CommandBinding.Command.MoveToMouse, CommandBinding.Command.ForceMoveToMouse)) {
				ClearQueuedAbility();
			}
			
			if (QueuedAbility.HasValue && QueuedAbility.Value.GetReason() == AbilityQueueReason.Range && IsInRangeToCast(QueuedAbility.Value)) {
				QueuedAbility.Value.GetAbility().OnCast(transform.gameObject, QueuedAbility.Value.GetTargetPoint(), QueuedAbility.Value.GetTargetUnit());
				ClearQueuedAbility();
			}

			if (abilityToQueue.HasValue) {
				QueuedAbility = abilityToQueue;
			}
		}

		private bool IsInRangeToCast(UnitAbility ability) {
			if (CommandStatus.IsActive(CommandBinding.Command.ForceCast)) {
				return true;
			}
			
			var target = Targeting.GetTargetedEnemy();
			var targetPoint = target.HasValue ? Maybe<Vector3>.Some(target.Value.transform.position) : MouseStatus.GetWalkableWorldPoint();

			if (!targetPoint.HasValue) {
				return false;
			}

			return Vector3.Distance(Utility.GetGroundPosition(transform.position), targetPoint.Value) <= ability.GetMaximumCastRange();
		}
		
		private bool IsInRangeToCast(QueuedPlayerAbility queuedAbility) {
			if (CommandStatus.IsActive(CommandBinding.Command.ForceCast)) {
				return true;
			}
			
			var target = queuedAbility.GetTargetUnit();
			var targetPoint = target.HasValue ? Maybe<Vector3>.Some(target.Value.transform.position) : queuedAbility.GetTargetPoint();

			if (!targetPoint.HasValue) {
				return false;
			}
			return Vector3.Distance(Utility.GetGroundPosition(transform.position), targetPoint.Value) <= queuedAbility.GetAbility().GetMaximumCastRange();
		}
		
		private Maybe<GameObject> GetTargetUnitIfInRange(Maybe<GameObject> targetUnit, float range) {
			if (!targetUnit.HasValue) {
				return targetUnit;
			}

			var position = Utility.GetGroundPosition(transform.position);
			if (Vector3.Distance(position, targetUnit.Value.transform.position) <= range) {
				return targetUnit;
			}
			return Maybe<GameObject>.None;
		}

		private Maybe<Vector3> GetClosestTargetPointInRange(Maybe<GameObject> targetUnit, Maybe<Vector3> targetPoint, float range) {
			if (targetUnit.HasValue) {
				targetPoint = Maybe<Vector3>.Some(targetUnit.Value.transform.position);
			}
			
			if (!targetPoint.HasValue) {
				return targetPoint;
			}
			return Maybe<Vector3>.Some(Vector3.MoveTowards(Utility.GetGroundPosition(transform.position), targetPoint.Value, range));
		}

		private Maybe<QueuedPlayerAbility> CreateQueuedAbility(PlayerAbility ability, AbilityQueueReason reason) {
			var targetUnit = Targeting.GetTargetedEnemy();
			if (targetUnit.HasValue) {
				return Maybe<QueuedPlayerAbility>.Some(new QueuedPlayerAbility(ability, targetUnit.Value, reason));
			}
			
			var targetPoint = MouseStatus.GetWalkableWorldPoint();
			if (targetPoint.HasValue) {
				return Maybe<QueuedPlayerAbility>.Some(new QueuedPlayerAbility(ability, targetPoint.Value, reason));
			}
			return Maybe<QueuedPlayerAbility>.None;
		}

		private void ClearQueuedAbility() {
			QueuedAbility = Maybe<QueuedPlayerAbility>.None;
		}

		public Maybe<QueuedPlayerAbility> GetQueuedAbility() {
			return QueuedAbility;
		}

		public Dictionary<CommandBinding.Command, PlayerAbility> GetLibrary() {
			return AbilityLibrary;
		}
	}
}