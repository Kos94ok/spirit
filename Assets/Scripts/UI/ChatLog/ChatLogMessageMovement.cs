using Misc;

namespace UI.ChatLog {
	public class ChatLogMessageMovement {
		private readonly float Distance;
		private readonly Timer Timer = new Timer();
		
		public ChatLogMessageMovement(float distance, float time) {
			Distance = distance;
			Timer.Start(time);
		}

		public float GetDistance() {
			return Distance;
		}

		public void Update() {
			Timer.Tick();
		}

		public bool IsDone() {
			return Timer.IsDone();
		}

		public float GetFraction() {
			return Timer.GetFraction();
		}
	}
}