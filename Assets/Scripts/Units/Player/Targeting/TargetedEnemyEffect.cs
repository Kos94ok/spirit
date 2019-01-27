using Misc;
using UnityEngine;

namespace Units.Player.Targeting {
	public class TargetedEnemyEffect : MonoBehaviour {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private UnitStats Stats;
		private GameObject SelectionCircle;
		
		private void Start() {
			Stats = GetComponent<UnitStats>();
			switch (Stats.Alliance) {
				case UnitAlliance.Ally:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Resource.TargetIndicatorAlly));
					break;
				case UnitAlliance.Enemy:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Resource.TargetIndicatorEnemy));
					break;
				default:
					SelectionCircle = (GameObject) Instantiate(Assets.Get(Resource.TargetIndicatorNeutral));
					break;
			}
			SelectionCircle.transform.localScale = new Vector3(Stats.SelectionRadius * 2, 0.001f, Stats.SelectionRadius * 2);
			SelectionCircle.transform.parent = transform;
			SelectionCircle.transform.localPosition = Vector3.zero;
		}

		private void Update() {
			SelectionCircle.transform.position = Utility.GetGroundPosition(transform.position);
		}

		private void OnDestroy() {
			Destroy(SelectionCircle);
		}
	}
}