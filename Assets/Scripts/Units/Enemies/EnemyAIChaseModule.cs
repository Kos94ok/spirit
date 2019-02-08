using System;
using Misc;
using UnityEngine;

namespace Units.Enemies {
	public class EnemyAIChaseModule {
		private readonly Func<Maybe<GameObject>> GetTargetEnemy;
		private readonly float EngageRange;
		private readonly float DisengageRange;
		
		private bool IsChasing;

		public EnemyAIChaseModule(Func<Maybe<GameObject>> getTargetEnemy, float engageRange, float disengageRange) {
			GetTargetEnemy = getTargetEnemy;
			EngageRange = engageRange;
			DisengageRange = disengageRange;
		}

		public void Update(Vector3 currentPosition) {
			var targetEnemy = GetTargetEnemy();
			if (!targetEnemy.HasValue) {
				IsChasing = false;
				return;
			}

			var targetPosition = targetEnemy.Value.transform.position;
			var distanceToTarget = Vector3.Distance(targetPosition, currentPosition);
			if (distanceToTarget <= EngageRange && !IsChasing && distanceToTarget < DisengageRange && !Utility.IsTargetObstructed(currentPosition, targetPosition)) {
				IsChasing = true;
			} else if (distanceToTarget >= DisengageRange) {
				IsChasing = false;
			}
		}

		public bool IsInCombat() {
			return IsChasing;
		}
	}
}
