using JetBrains.Annotations;
using Settings;
using UnityEngine;

namespace UI {
	[UsedImplicitly]
	public class KeyStatus {
		private KeyBinding keyBinding = AutowireFactory.GetInstanceOf<KeyBinding>();
	
		public bool IsPressed(KeyBinding.Action action) {
			var keyCode = keyBinding.Get(action);
			if (keyCode == null) {
				return false;
			}
			return Input.GetKey(keyCode.Value);
		}

		public bool IsReleased(KeyBinding.Action action) {
			return !IsPressed(action);
		}
	}
}
