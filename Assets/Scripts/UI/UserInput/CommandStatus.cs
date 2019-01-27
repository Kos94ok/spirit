using JetBrains.Annotations;
using Settings;
using UnityEngine;

namespace UI.UserInput {
	[UsedImplicitly]
	public class CommandStatus {
		private readonly CommandBinding CommandBinding = AutowireFactory.GetInstanceOf<CommandBinding>();
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();
	
		public bool IsActive(CommandBinding.Command command) {
			var commandBinding = CommandBinding.Get(command);
			if (commandBinding == null) {
				return false;
			}

			return commandBinding.IsKeyboard() && Input.GetKey(commandBinding.GetKeyCode())
				   || commandBinding.IsMouse() && MouseStatus.IsHoldDown(commandBinding.GetMouseButton());
		}

		public bool IsIssuedThisFrame(CommandBinding.Command command) {
			var commandBinding = CommandBinding.Get(command);
			if (commandBinding == null) {
				return false;
			}

			return commandBinding.IsKeyboard() && Input.GetKeyDown(commandBinding.GetKeyCode())
			       || commandBinding.IsMouse() && MouseStatus.IsClickedThisFrame(commandBinding.GetMouseButton());
		}

		public bool IsStoppedThisFrame(CommandBinding.Command command) {
			var commandBinding = CommandBinding.Get(command);
			if (commandBinding == null) {
				return false;
			}

			return commandBinding.IsKeyboard() && Input.GetKeyUp(commandBinding.GetKeyCode())
			       || commandBinding.IsMouse() && MouseStatus.IsReleasedThisFrame(commandBinding.GetMouseButton());
		}

		public bool IsInactive(CommandBinding.Command command) {
			return !IsActive(command);
		}
	}
}
