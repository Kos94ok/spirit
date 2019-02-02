using Misc;
using UnityEngine;

namespace Units.Common.HeightIndicator {
	public class HeightIndicator : MonoBehaviour {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

		private GameObject Indicator;
		private static readonly int Emission = Shader.PropertyToID("_EmissionColor");

		private void Start() {
			Indicator = new GameObject();
			Indicator = (GameObject) Instantiate(Assets.Get(Prefab.HeightIndicator), transform, true);
			var targetPosition = Utility.GetGroundPosition(transform.position);
			targetPosition.y += .1f;
			Indicator.transform.position = targetPosition;
			var stats = GetComponent<UnitStats>();

			switch (stats.Alliance) {
				case UnitAlliance.Enemy:
					Indicator.GetComponent<SkinnedMeshRenderer>().material.SetColor(Emission, new Color(1, 0, 1, 0));
					break;
			}
		}

		private void OnDestroy() {
			Destroy(Indicator);
		}
	}
}
