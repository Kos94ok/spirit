using Misc;
using Units.Common;
using UnityEngine;

namespace Units.Enemies {
	public abstract class EnemyAbility : UnitAbility {
		public void Cast(GameObject caster, GameObject targetUnit) {
			OnCast(caster, Maybe<Vector3>.None, Maybe<GameObject>.Some(targetUnit));
		}

		public void Cast(GameObject caster, Vector3 targetPosition) {
			OnCast(caster, Maybe<Vector3>.Some(targetPosition), Maybe<GameObject>.None);
		}

		public void Cancel() {
			OnCancel();
		}
	}
}