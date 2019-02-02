using Misc;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemies.ForgottenDweller {
    public class ForgottenDwellerAI : MonoBehaviour {
        private NavMeshAgent Agent;

        private const float WanderRange = 10f;
        private const float WanderPeriodMin = 4f;
        private const float WanderPeriodMax = 20f;
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
                WanderTimer.Start(Random.Range(WanderPeriodMin, WanderPeriodMax));
                var targetPosition = WanderAreaCenter + Random.insideUnitSphere * WanderRange;
                targetPosition.y = transform.position.y;
                Agent.SetDestination(targetPosition);
            }
        }
    }
}
