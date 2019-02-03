using Misc;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemies.ForgottenDweller {
	public class ForgottenDwellerAI : MonoBehaviour {
		private NavMeshAgent Agent;

		private const int TryLimit = 100;
		private const float WanderMaxRange = 10f;
		private const float WanderStepRange = 1f;
		private const float WanderPeriodMin = 2f;
		private const float WanderPeriodMax = 6f;
		private readonly Timer WanderTimer = new Timer();
		
		private Vector3 WanderAreaCenter;

		private void Start() {
			Agent = GetComponent<NavMeshAgent>();
			WanderTimer.Start(Random.Range(WanderPeriodMin, WanderPeriodMax));
			WanderAreaCenter = transform.position;
		}
		
		private void Update() {
			WanderTimer.Tick();
			if (WanderTimer.IsDone()) {
				var loopBreaker = 0;
				Vector3 targetPosition;
				var unitPosition = transform.position;
				do {
					targetPosition = unitPosition + Random.insideUnitSphere * WanderStepRange;
					targetPosition.y = unitPosition.y;
					loopBreaker += 1;
					if (loopBreaker > TryLimit) {
						WanderAreaCenter = transform.position;
						break;
					}
				} while (Vector3.Distance(WanderAreaCenter, targetPosition) > WanderMaxRange);
				Agent.SetDestination(targetPosition);
				WanderTimer.Start(Random.Range(WanderPeriodMin, WanderPeriodMax));
			}
		}
	}
}
