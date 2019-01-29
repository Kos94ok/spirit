using Misc;
using Units.Common.Lightning;
using UnityEngine;

namespace Units.Player.Combat.Abilities.PlayerBasicAttack {
	public class PlayerBasicAttack : PlayerAbility {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private Timer cooldown = new Timer();
		
		public override bool IsReady() {
			return cooldown.IsDone();
		}

		public override void Update() {
			cooldown.Tick();
		}

		public override void OnCast(GameObject caster, Maybe<Vector3> targetPoint, Maybe<GameObject> targetUnit) {
			var sourcePosition = caster.transform.position;
			var targetPosition = targetUnit.Value.GetComponent<UnitStats>().GetHitTargetPosition();

			var builder = new LightningAgent.Builder(sourcePosition, targetPosition)
				.SetAngularDeviation(50f)
				.SetSpeed(1000f)
				.SetBranchingChance(0.2f)
				.SetBranchingFactor(1.5f)
				.SetMaximumBranchDepth(3);
			builder.Create();
			builder.Create();
			cooldown.Start(0.5f);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumRange() {
			return 10f;
		}
	}
}