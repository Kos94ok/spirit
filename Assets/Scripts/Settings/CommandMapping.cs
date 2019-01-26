using UI;
using UI.UserInput;
using UnityEngine;

namespace Settings {
	public class CommandMapping {
		private readonly KeyCode? KeyCode;
		private readonly MouseStatus.Button? MouseButton;

		private CommandMapping(KeyCode keyCode) {
			KeyCode = keyCode;
			MouseButton = null;
		}

		private CommandMapping(MouseStatus.Button mouseButton) {
			MouseButton = mouseButton;
			KeyCode = null;
		}

		public bool IsKeyboard() {
			return KeyCode != null;
		}

		public bool IsMouse() {
			return MouseButton != null;
		}

		public KeyCode GetKeyCode() {
			Debug.Assert(KeyCode != null, "KeyCode != null");
			return KeyCode.Value;
		}

		public MouseStatus.Button GetMouseButton() {
			Debug.Assert(MouseButton != null, "MouseButton != null");
			return MouseButton.Value;
		}

		public static CommandMapping Keyboard(KeyCode keyCode) {
			return new CommandMapping(keyCode);
		}

		public static CommandMapping Mouse(MouseStatus.Button mouseButton) {
			return new CommandMapping(mouseButton);
		}
	}
}