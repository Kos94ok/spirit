using UnityEngine;

namespace Misc.TimerPool {
	public class TimerPoolAgent : MonoBehaviour {
		private readonly TimerPool TimerPool = AutowireFactory.GetInstanceOf<TimerPool>();

		private void Update() {
			TimerPool.TickAll();
		}
	}
}