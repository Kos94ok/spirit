using System.Collections;
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
			UpdateTargetPosition();
			StartCoroutine(DelayedPositionUpdate());
			var stats = GetComponent<UnitStats>();

			switch (stats.Alliance) {
				case UnitAlliance.Ally:
					Indicator.GetComponent<SkinnedMeshRenderer>().material.SetColor(Emission, new Color(0, 1, 0, 0));
					break;
				case UnitAlliance.Enemy:
					Indicator.GetComponent<SkinnedMeshRenderer>().material.SetColor(Emission, new Color(1, 0, 1, 0));
					break;
			}
		}
		
		private IEnumerator DelayedPositionUpdate() {
			yield return new WaitForSeconds(0.1f);
			UpdateTargetPosition();
		}
		
		private void UpdateTargetPosition() {
			var targetPosition = Utility.GetGroundPosition(transform.position);
			Indicator.transform.position = targetPosition;
		}

		private void OnDestroy() {
			Destroy(Indicator);
		}
	}
}
