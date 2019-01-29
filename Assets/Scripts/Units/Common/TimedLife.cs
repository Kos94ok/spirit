using UnityEngine;

namespace Units.Common {
	public class TimedLife : MonoBehaviour {

		public float Timer = 1.00f;
		public bool ShouldDestroy = true;
	
		private void Update () {
			Timer -= Time.deltaTime;
			if (Timer > 0.00f) {
				return;
			}

			if (ShouldDestroy) {
				Destroy(gameObject);
			} else {
				gameObject.SetActive(false);
			}
		}
	}
}
