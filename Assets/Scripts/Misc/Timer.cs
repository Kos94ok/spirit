
namespace Misc {
	public class Timer {
		private float counter;
		private float maxTime;
		private bool isTicking;

		public void Start(float time) {
			counter = 0.00f;
			maxTime = time;
			isTicking = true;
		}

		public void Reset() {
			counter = 0.00f;
		}

		public void Stop() {
			isTicking = false;
		}
		
		public bool Tick(float delta) {
			if (!isTicking) {
				return false;
			}
			
			counter += delta;
			if (counter < maxTime) {
				return false;
			}
			
			isTicking = false;
			return true;
		}

		public bool IsDone() {
			return counter >= maxTime || !isTicking;
		}

		public bool IsTicking() {
			return isTicking;
		}

		public float GetFraction() {
			return counter / maxTime;
		}
	}
}