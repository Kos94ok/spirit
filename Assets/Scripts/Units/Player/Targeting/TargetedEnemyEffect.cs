using Misc;
using UnityEngine;

namespace Units.Player.Targeting {
	public class TargetedEnemyEffect : MonoBehaviour {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private GameObject SelectionCircle;
		
		private void Start() {
			var stats = GetComponent<UnitStats>();
			switch (UnitRelationship.GetRelationship(UnitAlliance.Player, stats.Alliance)) {
				case UnitRelationship.Ally:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Prefab.TargetIndicatorAlly));
					break;
				case UnitRelationship.Enemy:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Prefab.TargetIndicatorEnemy));
					break;
				default:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Prefab.TargetIndicatorNeutral));
					break;
			}
			SelectionCircle.transform.localScale = new Vector3(stats.SelectionRadius * 2, 0.001f, stats.SelectionRadius * 2);
			var parentTransform = transform;
			SelectionCircle.transform.parent = parentTransform;
			SelectionCircle.transform.position = Utility.GetGroundPosition(parentTransform.position);
		}

		private void Update() {
			SelectionCircle.transform.position = Utility.GetGroundPosition(transform.position);
		}

		private void OnDestroy() {
			Destroy(SelectionCircle);
		}
	}
}