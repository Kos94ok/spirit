using Misc;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemies.AwakenedDweller {
	public class AwakenedDwellerAI : MonoBehaviour, IEnemyAI {
		private GameObject Player;
		private UnitStats Stats;
		private UnitStats PlayerStats;
		private NavMeshAgent Agent;

		private EnemyAIChaseModule ChaseModule;
		private readonly AwakenedDwellerBasicAttack BasicAttack = new AwakenedDwellerBasicAttack();

		private const float EngageRange = 8f;
		private const float DisengageRange = 12f;
		
		private void Start() {
			ChaseModule = new EnemyAIChaseModule(GetTargetEnemy, EngageRange, DisengageRange);
			
			Player = GameObject.FindGameObjectWithTag("Player");
			Stats = GetComponent<UnitStats>();
			PlayerStats = Player.GetComponent<UnitStats>();
			Agent = GetComponent<NavMeshAgent>();
		}
		
		private void Update() {
			BasicAttack.Update();
			ChaseModule.Update(transform.position);
			if (!ChaseModule.IsInCombat()) {
				return;
			}

			var position = transform.position;
			var targetEnemy = GetTargetEnemy().Value;
			var enemyPosition = targetEnemy.transform.position;
			if (BasicAttack.IsReady() && Vector3.Distance(position, enemyPosition) <= BasicAttack.GetMaximumCastRange()
			                          && !Utility.IsTargetObstructed(position, enemyPosition)) {
				Agent.SetDestination(position);
				BasicAttack.Cast(gameObject, targetEnemy);
			} else if (BasicAttack.IsReady()) {
				Agent.SetDestination(targetEnemy.transform.position);
			}
		}

		private Maybe<GameObject> GetTargetEnemy() {
			return Maybe<GameObject>.Some(Player);
		}
		
		public void OnHit(float damage, Maybe<GameObject> source) {}
	}
}
