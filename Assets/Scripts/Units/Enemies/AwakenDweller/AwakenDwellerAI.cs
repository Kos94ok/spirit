using Misc;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemies.AwakenDweller {
	public class AwakenDwellerAI : MonoBehaviour {
		private GameObject Player;
		private UnitStats Stats;
		private UnitStats PlayerStats;
		private NavMeshAgent Agent;

		private void Start() {
			Player = GameObject.FindGameObjectWithTag("Player");
			Stats = GetComponent<UnitStats>();
			PlayerStats = Player.GetComponent<UnitStats>();
			Agent = GetComponent<NavMeshAgent>();
		}
		
		private void Update() {
		}
	}
}
