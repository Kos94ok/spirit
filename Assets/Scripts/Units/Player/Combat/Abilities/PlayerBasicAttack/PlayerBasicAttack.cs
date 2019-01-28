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
			LightningAgent.Create(caster.transform.position, targetUnit.Value.transform.position, 0.0f);
			cooldown.Start(0.2f);
		}

		public override int GetTargetType() {
			return AbilityTargetType.Unit;
		}

		public override float GetMaximumRange() {
			return 2f;
		}
	}
}