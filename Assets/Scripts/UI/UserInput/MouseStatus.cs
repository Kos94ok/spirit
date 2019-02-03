using System;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Misc;
using UnityEngine;

namespace UI.UserInput {
	[UsedImplicitly]
	public class MouseStatus {
		public enum Button {
			Left,
			Right,
			Middle,
		}
		
		public Maybe<Vector3> GetWalkableWorldPoint() {
			if (Camera.main == null) {
				return Maybe<Vector3>.None;
			}
			
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  
			if (Physics.Raycast(ray, out hit, 1000.00f, Layers.Walkable)) {
				return Maybe<Vector3>.Some(hit.point);
			}
			return Maybe<Vector3>.None;
		}

		public Maybe<GameObject> GetHoveredEnemy() {
			if (Camera.main == null) {
				return Maybe<GameObject>.None;
			}
			
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  
			if (Physics.Raycast(ray, out hit, 1000.00f, Layers.EnemyHitbox)) {
				return Maybe<GameObject>.Some(hit.transform.parent.gameObject);
			}
			return Maybe<GameObject>.None;
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
