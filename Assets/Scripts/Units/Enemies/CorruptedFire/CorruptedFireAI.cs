using System.Collections.Generic;
using Misc;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemies.CorruptedFire {
	public class CorruptedFireAI : EnemyAI {
		private const float ContactDamage = 1;
		private const float ChargeDamage = 1;
		private const float ContactDamageRadius = .5f;
		private const float ChargeDamageRadius = .5f;

		private const float ChargeRange = 5;
		private const float ChargeWarmUp = 1.75f;
		private const float ChargeCooldown = 2;
		private const float ChargeSpeed = 25;
		private const float ChargeOvershoot = 2;

		private const float CombatEngageRange = 10;
		private const float CombatDisengageRange = 15;

		private bool IsChasing;
		private bool ChargeWarmingUp;
		private bool ChargeInProgress;

		private float AgentRadius;
		private float MovementSpeed;
		private float MovementAcceleration;
		private float MovementAngularSpeed;

		private readonly Timer ChargeWarmUpTimer = new Timer();
		private readonly Timer ChargeCooldownTimer = new Timer();
		private Vector3 ChargeTargetPoint;
		private GameObject ChargeCastPe;
		private readonly List<GameObject> ChargeHitObjectsList = new List<GameObject>();

		private GameObject Player;
		private UnitStats Stats;
		private UnitStats PlayerStats;
		private NavMeshAgent Agent;

		private void Start() {
			Player = GameObject.FindGameObjectWithTag("Player");
			Stats = GetComponent<UnitStats>();
			PlayerStats = Player.GetComponent<UnitStats>();
			Agent = GetComponent<NavMeshAgent>();
			Agent.updateRotation = true;

			AgentRadius = Agent.radius;
			MovementSpeed = Agent.speed;
			MovementAcceleration = Agent.acceleration;
			MovementAngularSpeed = Agent.angularSpeed;
		}

		public override void OnHit(float damage, GameObject source = null) {
			if (ChargeInProgress) {
				Stats.DealDamage(damage * 9, source);
			}
		}

		private void Update() {
			ChargeCooldownTimer.Tick();
			
			var distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
			if (distanceToPlayer <= CombatEngageRange && !IsChasing || distanceToPlayer < CombatDisengageRange && IsChasing || ChargeInProgress) {
				IsChasing = true;
				if (!ChargeWarmingUp && !ChargeInProgress) {
					Agent.SetDestination(Player.transform.position);
				}

				if (ChargeCooldownTimer.IsDone()) {
					Agent.speed = MovementSpeed;
				}

				// Contact damage
				if (Vector3.Distance(Player.transform.position, transform.position) <= ContactDamageRadius) {
					PlayerStats.DealDamage(ContactDamage * Time.deltaTime, gameObject);
					var playerHitPe = (GameObject) Instantiate(Resources.Load("CorruptedFirePEOnPlayerHitSmall"));
					playerHitPe.transform.position = Player.transform.position;
				}

				// Start charge ability warm-up
				if (distanceToPlayer <= ChargeRange && !ChargeWarmingUp && !ChargeInProgress && ChargeCooldownTimer.IsDone()
						&& !Utility.IsTargetObstructed(transform.position, Player.transform.position, AgentRadius)) {
					ChargeWarmingUp = true;
					Agent.acceleration = 1000.00f;
					Agent.angularSpeed = 1000.00f;
					Agent.SetDestination(transform.position);
					ChargeWarmUpTimer.Start(ChargeWarmUp);
					ResetHitObjectsList();

					ChargeCastPe = (GameObject) Instantiate(Resources.Load("CorruptedFirePEChargeCast"));
					ChargeCastPe.transform.position = transform.GetChild(0).position;
				}

			
				// Charge warm up
				ChargeWarmUpTimer.Tick();
				if (ChargeWarmingUp && ChargeWarmUpTimer.IsDone()) {
					if (Utility.IsTargetObstructed(transform.position, Player.transform.position, AgentRadius)) {
						StopCharge();
						ChargeCastPe.GetComponent<ParticleSystem>().enableEmission = false;
					} else {
						Agent.acceleration = 0.00f;
						Agent.angularSpeed = 0.0f;
						Agent.speed = 0.00f;

						var position = transform.position;
						var playerPosition = Player.transform.position;
						playerPosition.y = position.y;
						var targetDirection = (playerPosition - position).normalized;
						ChargeTargetPoint = Vector3.MoveTowards(position, playerPosition, ChargeRange) + targetDirection * ChargeOvershoot;

						ChargeWarmingUp = false;
						ChargeInProgress = true;
						Agent.SetDestination(ChargeTargetPoint);
					}
				}

				// Charge in progress
				if (ChargeInProgress) {
					var oldPos = transform.position;
					var distanceToMove = ChargeSpeed * Time.deltaTime;

					Agent.Move((ChargeTargetPoint - oldPos).normalized * distanceToMove);

					if (!ChargeHitObjectsList.Contains(Player) && Math.GetDistance2D(transform.position, Player.transform.position) <= ChargeDamageRadius) {
						Player.GetComponent<UnitStats>().DealDamage(ChargeDamage, gameObject);
						ChargeHitObjectsList.Add(Player);
						var playerBlood = (GameObject) Instantiate(Resources.Load("CorruptedFirePEOnPlayerHit"));
						var playerPosition = Player.transform.position;
						var adjustedChargeTargetPoint = ChargeTargetPoint;
						adjustedChargeTargetPoint.y = playerPosition.y;
						playerBlood.transform.position = playerPosition;
						playerBlood.transform.Rotate(Vector3.down, 180 + Math.GetAngle(playerPosition, adjustedChargeTargetPoint));
					}

					var newPos = transform.position;
					if (Vector3.Distance(ChargeTargetPoint, newPos) <= distanceToMove || Vector3.Distance(oldPos, newPos) < distanceToMove / 2) {
						StopCharge();
					}
				}

			} else if (distanceToPlayer >= CombatDisengageRange) {
				IsChasing = false;
			}
		}


		private void StopCharge() {
			Agent.SetDestination(transform.position);
			ChargeWarmingUp = false;
			ChargeInProgress = false;
			ChargeWarmUpTimer.Stop();
			ChargeCooldownTimer.Start(ChargeCooldown);

			Agent.speed = MovementSpeed / 2;
			Agent.acceleration = MovementAcceleration;
			Agent.angularSpeed = MovementAngularSpeed;
		}

		private void ResetHitObjectsList()
		{
			ChargeHitObjectsList.Clear();
		}
	}
}
