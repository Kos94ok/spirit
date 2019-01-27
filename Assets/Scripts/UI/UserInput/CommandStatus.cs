using System.Linq;
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

		public bool IsAnyActive(params CommandBinding.Command[] commands) {
			return commands.Any(IsActive);
		}
		
		public bool IsAllActive(params CommandBinding.Command[] commands) {
			return commands.All(IsActive);
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

		public bool IsAnyStoppedThisFrame(params CommandBinding.Command[] commands) {
			return commands.Any(IsStoppedThisFrame);
		}

		public bool IsInactive(CommandBinding.Command command) {
			return !IsActive(command);
		}
	}
}
