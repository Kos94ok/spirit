using Misc;
using UnityEngine;

namespace Units.Common.Lightning {
	public class LightningFragment : MonoBehaviour {

		private LightningAgent Parent;
		private bool IsExpanded;
		private int MaximumDepth;
		private Vector3 TargetPoint;
		private readonly Timer DelayTimer = new Timer();
		private readonly Timer DestroyTimer = new Timer();

		public void Init(LightningAgent parent, Vector3 targetPoint, int maxDepth, float delay) {
			Parent = parent;
			MaximumDepth = maxDepth;
			TargetPoint = targetPoint;
			DelayTimer.Start(delay);
			DestroyTimer.Start(0.05f);
		}

		private void Start() {
			var spawnPosition = transform.GetChild(1).position;
			
			if (MaximumDepth > 0 && Vector3.Distance(spawnPosition, TargetPoint) > 0.05f) {
				Parent.AddFragment(spawnPosition, MaximumDepth - 1);
				if (Random.Range(0f, 1f) < 0.00f) {
					Parent.AddFragment(spawnPosition, Mathf.Min(2, MaximumDepth));
				}

				if (Random.Range(0f, 1f) < 0.0f) {
					Parent.AddFragment(spawnPosition, Mathf.Min(1, MaximumDepth));
				}
			}
		}

		private void Update() {
			DestroyTimer.Tick();
			if (DestroyTimer.IsDone()) {
				transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
				DestroyTimer.Start(1000f);
			}

			/*if (IsExpanded) {
				return;
			}*/
			
			/*DelayTimer.Tick();
			if (!DelayTimer.IsDone()) {
				return;
			}*/
			
			/*if (MaximumDepth > 0) {
				IsExpanded = true;
				Parent.AddFragment(transform.GetChild(1).position, MaximumDepth - 1);
				if (Random.Range(0f, 1f) < 0.50f) {
					Parent.AddFragment(transform.GetChild(1).position, Mathf.Min(2, MaximumDepth));
				}
				if (Random.Range(0f, 1f) < 0.20f) {
					Parent.AddFragment(transform.GetChild(1).position, Mathf.Min(1, MaximumDepth));
				}
			}*/
		}
	}
}