using Misc;
using UnityEngine;

namespace Units.Common {
	public class HeightIndicator : MonoBehaviour {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private GameObject Indicator;
		private void Start() {
			Indicator = new GameObject();
			Indicator = Instantiate(Assets.Get(Prefab.HeightIndicator)) as GameObject;
		}
		private void Update() {
			Indicator.transform.position = Utility.GetGroundPosition(transform.position);
		}
		
		private void OnDestroy() {
			Destroy(Indicator);
		}
	}
}
