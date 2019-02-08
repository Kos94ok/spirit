
using System;
using UnityEngine;

namespace Misc {
	public class Timer {
		private float Counter;
		private float MaxTime;
		private bool IsTicking;
		private bool IsForever;
		private Maybe<Action> OnDone = Maybe<Action>.None;
		private readonly TimerPool.TimerPool TimerPool = AutowireFactory.GetInstanceOf<TimerPool.TimerPool>();

		public void Start(float time) {
			Counter = 0.00f;
			MaxTime = time;
			IsTicking = true;
			TimerPool.Register(this);
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
			TimerPool.Unregister(this);
		}
		
		public void Tick() {
			if (!IsTicking) {
				return;
			}
			
			Counter += Time.deltaTime;
			if (Counter >= MaxTime && OnDone.HasValue) {
				OnDone.Value();
			}
			
			if (Counter >= MaxTime && IsForever) {
				Counter -= MaxTime;
			} else if (Counter >= MaxTime) {
				IsTicking = false;
				TimerPool.Unregister(this);
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

		public void SetOnDoneAction(Action action) {
			OnDone = Maybe<Action>.Some(action);
		}
	}
}