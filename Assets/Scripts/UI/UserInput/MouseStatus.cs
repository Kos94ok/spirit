using JetBrains.Annotations;
using UnityEngine;

namespace UI.UserInput {
	[UsedImplicitly]
	public class MouseStatus {
		public enum Button {
			Left,
			Right,
			Middle,
		}
		
		public Vector3? GetWorldPointForWalkableLayer() {
			if (Camera.main == null) {
				return Vector3.zero;
			}
			
			RaycastHit hit;
			const int walkableLayer = 1 << 9;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  
			if (Physics.Raycast(ray, out hit, 1000.00f, walkableLayer)) {
				return hit.point;
			}
			return null;
		}

		public bool IsHoldDown(Button mouseButton) {
			var code = GetMouseButtonCode(mouseButton);
			return Input.GetMouseButton(code);
		}

		public bool IsClickedThisFrame(Button mouseButton) {
			var code = GetMouseButtonCode(mouseButton);
			return Input.GetMouseButtonDown(code);
		}

		public bool IsReleasedThisFrame(Button mouseButton) {
			var code = GetMouseButtonCode(mouseButton);
			return Input.GetMouseButtonUp(code);
		}

		private int GetMouseButtonCode(Button mouseButton) {
			switch (mouseButton) {
				case Button.Left:
					return 0;
				case Button.Right:
					return 1;
				case Button.Middle:
					return 2;
			}
			return 0;
		}
	}
}
