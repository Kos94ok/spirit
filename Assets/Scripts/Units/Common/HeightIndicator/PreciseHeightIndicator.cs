using Misc;
using UnityEngine;

namespace Units.Common.HeightIndicator {
	public class PreciseHeightIndicator : MonoBehaviour {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

		private GameObject Indicator;

		private void Start() {
			Indicator = new GameObject();
			Indicator = (GameObject) Instantiate(Assets.Get(Prefab.HeightIndicator));
		}
		private void Update() {
			Indicator.transform.position = Utility.GetGroundPosition(transform.position);				
		}

		private void OnDestroy() {
			Destroy(Indicator);
		}
	}
}
