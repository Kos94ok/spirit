using UnityEngine;

namespace Units.Player.Movement {
	public class PlayerTargetPositionIndicator : MonoBehaviour {
		public void Show() {
			transform.gameObject.SetActive(true);
		}
		public void Hide() {
			transform.gameObject.SetActive(false);
		}
		public void MoveTo(Vector3 target) {
			transform.position = target;
		}
	}
}
