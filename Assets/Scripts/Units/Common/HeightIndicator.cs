using UnityEngine;

namespace Units.Common {
	public class HeightIndicator : MonoBehaviour {
		private GameObject Indicator;
		private void Start() {
			Indicator = new GameObject();
			Indicator = Instantiate(Resources.Load("HeightIndicator")) as GameObject;
		}
		private void Update() {
			Indicator.transform.position = GetGroundPosition();
		}
		
		private Vector3 GetGroundPosition() {
			RaycastHit hit;
			const int walkableLayerMask = 1 << 9;
			var ray = new Ray(transform.position, Vector3.down);

			if (Physics.Raycast(ray, out hit, 1000.00f, walkableLayerMask)) {
				return hit.point;
			}

			return Vector3.zero;
		}
	}
}
