
using UnityEngine;

namespace Misc {
	public class Timer {
		private float Counter;
		private float MaxTime;
		private bool IsTicking;
		private bool IsForever;

		public void Start(float time) {
			Counter = 0.00f;
			MaxTime = time;
			IsTicking = true;
		}

		public void StartForever(float time) {
			Start(time);
			IsForever = true;
		}

		public void Reset() {
			Counter = 0.00f;
		}

		public void Stop() {
			IsTicking = false;
			IsForever = false;
		}
		
		public void Tick() {
			if (!IsTicking) {
				return;
			}
			
			Counter += Time.deltaTime;
			if (Counter >= MaxTime && IsForever) {
				Counter -= MaxTime;
			} else if (Counter >= MaxTime) {
				IsTicking = false;
			}
		}

		public void TickIf(bool condition) {
			if (!condition) {
				return;
			}

			Tick();
		}

		public bool IsDone() {
			return Counter >= MaxTime || !IsTicking;
		}

		public bool IsRunning() {
			return IsTicking;
		}

		public bool IsStopped() {
			return !IsTicking;
		}

		public float GetFraction() {
			return Counter / MaxTime;
		}		
	}
}