
using UnityEngine;

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
		
		public void Tick() {
			if (!isTicking) {
				return;
			}
			
			counter += Time.deltaTime;
			if (counter < maxTime) {
				return;
			}
			
			isTicking = false;
		}

		public void TickIf(bool condition) {
			if (!condition) {
				return;
			}

			Tick();
		}

		public bool IsDone() {
			return counter >= maxTime || !isTicking;
		}

		public bool IsTicking() {
			return isTicking;
		}

		public bool IsStopped() {
			return !isTicking;
		}

		public float GetFraction() {
			return counter / maxTime;
		}
	}
}