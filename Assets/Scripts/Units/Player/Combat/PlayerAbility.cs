using Misc;
using Settings;
using UI.UserInput;
using Units.Common;
using UnityEngine;

namespace Units.Player.Combat {
	public abstract class PlayerAbility : UnitAbility {
		protected readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		public override bool IsTargetingPoint() {
			return (GetTargetType() & AbilityTargetType.Point) > 0 
			       || (GetTargetType() & AbilityTargetType.Unit) > 0 && CommandStatus.IsActive(CommandBinding.Command.ForceCast);
		}
	}
}