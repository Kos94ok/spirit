using Misc;
using Settings;
using UI.UserInput;
using Units.Buffs;
using Units.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace Units {
	public enum UnitAlliance {
		Player,
		Forgotten,
		Corruption,
	}

	public enum ShieldsBehaviour {
		PassiveGeneration,
		PassiveDecay,
	}

	public enum ShieldsRegenerationType {
		Always,
		InCombat,
		OutOfCombatInstant,
		OutOfCombatWithDelay,
		OutOfCombatWhenNotUsed,
		Never,
	}

	public enum RegenerationType {
		Always,
		WithDelay,
		WhenNotUsed,
		Never,
	}

	public enum CombatState {
		In,
		Out,
	}

	public sealed class UnitStats : MonoBehaviour {
		public string DebugName;
		public UnitAlliance Alliance = UnitAlliance.Forgotten;
		public float TimeUntilOutOfCombat = 3.00f;

		public float Health = 100.00f;
		public float HealthMax = 100.00f;
		public float HealthRegen = 1.00f;
		public float HealthRegenDelay = 1.00f;
		public RegenerationType HealthRegenMode = RegenerationType.Always;

		public float Mana = 100.00f;
		public float ManaMax = 100.00f;
		public float ManaRegen = 10.00f;
		public float ManaRegenDelay = 1.00f;
		public RegenerationType ManaRegenMode = RegenerationType.WhenNotUsed;

		public float Shields;
		public float ShieldsMax = 1000.00f;
		public float ShieldsRegen = 10.00f;
		public float ShieldsRegenDelay = 3.00f;
		public ShieldsBehaviour ShieldsPassiveMode = ShieldsBehaviour.PassiveDecay;
		public ShieldsRegenerationType ShieldsRegenMode = ShieldsRegenerationType.OutOfCombatWithDelay;

		public float SelectionRadius = 1.00f;
		public Vector3 CenterOffset = Vector3.zero;

		private bool IsConfirmedDead;

		private bool HealthUsedThisFrame;
		private bool ManaUsedThisFrame;
		private bool ShieldsAffectedThisFrame;
		private CombatState CombatState = CombatState.Out;
		private readonly Timer HealthRegenTimer = new Timer();
		private readonly Timer ManaRegenTimer = new Timer();
		private readonly Timer ShieldsRegenTimer = new Timer();
		private readonly Timer CombatTimer = new Timer();

		private IEnemyAI EnemyAIController;

		[HideInInspector]
		public BuffController Buffs;

		private void Start() {
			EnemyAIController = GetComponent<IEnemyAI>();
			Buffs = gameObject.AddComponent<BuffController>();
		}
		private void Update() {
			if (!IsAlive()) {
				return;
			}
		
			// Basic health regeneration
			if (HealthRegenMode == RegenerationType.Always
			    || HealthRegenMode == RegenerationType.WithDelay && HealthRegenTimer.IsDone()
			    || HealthRegenMode == RegenerationType.WhenNotUsed && !HealthUsedThisFrame) {
				HealDamage(HealthRegen * Time.deltaTime);
			}
			// Basic mana regeneration
			if (ManaRegenMode == RegenerationType.Always
			    || ManaRegenMode == RegenerationType.WithDelay && ManaRegenTimer.IsDone()
			    || ManaRegenMode == RegenerationType.WhenNotUsed && !ManaUsedThisFrame) {
				RestoreMana(ManaRegen * Time.deltaTime);
			}
			// Shield passive mode
			if (ShieldsRegenMode == ShieldsRegenerationType.Always
			    || IsInCombat() && ShieldsRegenMode == ShieldsRegenerationType.InCombat
			    || !IsInCombat() && ShieldsRegenMode == ShieldsRegenerationType.OutOfCombatInstant
			    || !IsInCombat() && ShieldsRegenMode == ShieldsRegenerationType.OutOfCombatWithDelay && ShieldsRegenTimer.IsDone()
			    || !IsInCombat() && ShieldsRegenMode == ShieldsRegenerationType.OutOfCombatWhenNotUsed && !ShieldsAffectedThisFrame) {
				switch (ShieldsPassiveMode) {
					case ShieldsBehaviour.PassiveGeneration:
						GainShields(ShieldsRegen * Time.deltaTime);
						break;
					case ShieldsBehaviour.PassiveDecay:
						DrainShields(ShieldsRegen * Time.deltaTime);
						break;
				}
			}
			// Combat mode
			if (IsInCombat() && CombatTimer.IsDone()) {
				OnCombatLeave();
			}
			HealthUsedThisFrame = false;
			ManaUsedThisFrame = false;
			ShieldsAffectedThisFrame = false;
		}

		public bool HasShields(float amount, float buffer = 1.00f) {
			return Shields >= amount + buffer;
		}
	
		public float DrainShields(float amount) {
			if (amount <= 0f)
				return 0f;

			var damageOverflow = 0f;
			if (HasShields(amount, 0f)) {
				Shields -= amount;
			} else {
				damageOverflow = amount - Shields;
				Shields = 0f;
			}
			ShieldsAffectedThisFrame = true;
			return damageOverflow;
		}
	
		public void GainShields(float amount) {
			Shields += amount;
			if (Shields > ShieldsMax) {
				Shields = ShieldsMax;
			}

			if (ShieldsRegenMode == ShieldsRegenerationType.OutOfCombatWithDelay) {
				ShieldsRegenTimer.Start(ShieldsRegenDelay);
			}
			ShieldsAffectedThisFrame = true;
		}
	
		public bool HasHealth(float amount, float buffer = 1.00f) {
			return Health >= amount + buffer || Buffs.Has(Buff.Invulnerable) || Buffs.Has(Buff.GodMode);
		}

		public float DealDamage(float amount) {
			return DealDamageInternal(amount, Maybe<GameObject>.None, true);
		}
		public float DealDamage(float amount, Maybe<GameObject> source) {
			return DealDamageInternal(amount, source, true);
		}

		public float DealDamageIgnoreOnHit(float amount, Maybe<GameObject> source) {
			return DealDamageInternal(amount, source, false);
		}
		
		private float DealDamageInternal(float amount, Maybe<GameObject> source, bool triggerOnHit) {
			if (IsDead() || amount <= 0.00f || Buffs.Has(Buff.Invulnerable) || Buffs.Has(Buff.GodMode))
				return 0f;

			EngageCombat();

			var overflow = amount;
			if (Buffs.Has(Buff.ManaShield)) {
				overflow = DrainMana(overflow);
			}
			overflow = DrainShields(overflow);
			Health -= overflow;
			if (Health <= 0f) {
				overflow = -Health;
				Kill();
			} else {
				overflow = 0f;
				if (EnemyAIController != null && triggerOnHit) {
					EnemyAIController.OnHit(amount, source);
				}
			}

			if (HealthRegenMode == RegenerationType.WithDelay) {
				HealthRegenTimer.Start(HealthRegenDelay);
			}
			HealthUsedThisFrame = true;
			return overflow;
		}
	
		public float HealDamage(float amount) {
			var overflow = Mathf.Max(0f, amount - (HealthMax - Health));
			Health += amount;
			Health = Mathf.Min(Health, HealthMax);
			return overflow;
		}
	
		public bool HasMana(float amount) {
			return Mana >= amount || Buffs.Has(Buff.UnlimitedMana) || Buffs.Has(Buff.GodMode);
		}
	
		public float DrainMana(float amount) {
			if (amount <= 0f || Buffs.Has(Buff.UnlimitedMana) || Buffs.Has(Buff.GodMode))
				return 0f;

			var overflow = 0f;
			if (HasMana(amount)) {
				Mana -= amount;
			} else {
				overflow = amount - Mana;
				Mana = 0f;
			}

			if (ManaRegenMode == RegenerationType.WithDelay) {
				ManaRegenTimer.Start(ManaRegenDelay);
			}

			ManaUsedThisFrame = true;
			return overflow;
		}
	
		public float RestoreMana(float amount) {
			var overflow = Mathf.Max(0f, amount - (ManaMax - Mana));
			Mana += amount;
			Mana = Mathf.Min(Mana, ManaMax);
			return overflow;
		}

		public bool IsAlive() {
			return Health > 0f;
		}

		public bool IsDead() {
			return !IsAlive();
		}
	
		public void Kill() {
			if (IsConfirmedDead) {
				return;
			}

			Health = 0.00f;
			IsConfirmedDead = true;
			if (!CompareTag("Player")) {
				var navMeshAgent = GetComponent<NavMeshAgent>();
				if (navMeshAgent != null) {
					Destroy(navMeshAgent);
				}

				var animator = GetComponent<Animator>();
				if (animator != null) {
					Destroy(animator);
				}

				var unitDeath = GetComponent<UnitDeath>();
				if (unitDeath != null) {
					unitDeath.OnDeath();
				}

				var corpseCleanUp = gameObject.AddComponent<CorpseCleanUp>();
				corpseCleanUp.SetTimer(15.00f, 0.75f);
			}
		}
	
		public void ApplyForce(Vector3 force, ForceMode mode) {
			if (IsAlive()) {
				return;
			}

			var unitDeath = GetComponent<UnitDeath>();
			if (unitDeath != null) {
				unitDeath.ApplyForce(force, mode);
			}
		}
	
		public bool IsInCombat() {
			return CombatState == CombatState.In;
		}

		public bool IsInRelationship(UnitAlliance otherAlliance, int relationship) {
			return (UnitRelationship.GetRelationship(Alliance, otherAlliance) & relationship) > 0;
		}
	
		public void EngageCombat() {
			if (!IsInCombat()) {
				OnCombatEnter();
			}
			CombatTimer.Start(TimeUntilOutOfCombat);
		}
	
		public void OnCombatEnter() {
			CombatState = CombatState.In;
		}
	
		public void OnCombatLeave() {
			CombatState = CombatState.Out;
		}

		public Vector3 GetHitTargetPosition() {
			return transform.position + CenterOffset;
		}
	}
}