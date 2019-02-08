
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Misc.TimerPool {
	[UsedImplicitly]
	public class TimerPool {
		private bool IsInitialized;
		private readonly List<Timer> RegisteredTimers = new List<Timer>();
		private readonly List<Timer> ExpiredTimers = new List<Timer>();

		private void Initialize() {
			IsInitialized = true;
			var agent = new GameObject();
			agent.AddComponent<TimerPoolAgent>();
		}
		
		public void Register(Timer timer) {
			if (!IsInitialized) {
				Initialize();
			}
			
			RegisteredTimers.Add(timer);
		}
		
		public void Unregister(Timer timer) {
			ExpiredTimers.Add(timer);
		}
		
		public void TickAll() {
			foreach (var expiredTimer in ExpiredTimers) {
				RegisteredTimers.Remove(expiredTimer);
			}
			ExpiredTimers.Clear();
			foreach (var timer in RegisteredTimers) {
				timer.Tick();
			}
		}
	}
}