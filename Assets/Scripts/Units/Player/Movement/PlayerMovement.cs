using Misc;
using Settings;
using UI.UserInput;
using Units.Buffs;
using Units.Player.Combat;
using Units.Player.Targeting;
using UnityEngine;

namespace Units.Player.Movement {
	public class PlayerMovement : MonoBehaviour {
		private const float MaximumSpeed = 2.15f;
		private const float Acceleration = 5.00f;
		private const float AngularAcceleration = 10.00f;
		private const float SprintMaximumSpeed = 3.50f;
		private const float SprintAcceleration = 15.00f;
		private const float DecelerationDistance = 1.00f;
		private const float SprintManaCost = 5.00f;
		private const float SprintManaBuffer = 5.00f;

		private float MovementSpeed;
		private float LastDecelerationModifier;
		private Vector3 MovementDirection;
		private Maybe<Vector3> TargetPosition = Maybe<Vector3>.None;
		private PlayerTargetPositionIndicator TargetPositionIndicator;
		private bool IsSprinting;

		private const float ExpectedFloatingHeight = 0.6f;

		private float LastNonDeceleratingSpeed = MaximumSpeed;
		private Vector3 LastGroundedPosition;

		private UnitStats Stats;
		private PlayerCombat Combat;
		private PlayerTargeting Targeting;
		private PlayerEquipment Equipment;
		private CharacterController MovementController;
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		private void Start() {
			Stats = GetComponent<UnitStats>();
			Combat = GetComponent<PlayerCombat>();
			Targeting = GetComponent<PlayerTargeting>();
			Equipment = GetComponent<PlayerEquipment>();
			MovementController = GetComponent<CharacterController>();
			var targetPositionIndicatorAgent = (GameObject) Instantiate(Assets.Get(Prefab.TargetIndicatorPosition));
			TargetPositionIndicator = targetPositionIndicatorAgent.GetComponent<PlayerTargetPositionIndicator>();
		}

		private void Update() {
			Update_Movement();
			Update_Blink();
		}
		private void Update_Movement() {
			if (Stats.IsDead()) { return; }

			var isSprintKeyDown = CommandStatus.IsActive(CommandBinding.Command.Sprint);
			if (isSprintKeyDown && TargetPosition.HasValue && Stats.HasMana(SprintManaBuffer)) {
				IsSprinting = true;
			} else if (!isSprintKeyDown || !TargetPosition.HasValue || !Stats.HasMana(SprintManaCost * Time.deltaTime)) {
				IsSprinting = false;
			}

			var acceleration = Acceleration;
			var maximumSpeed = MaximumSpeed;
			if (IsSprinting) {
				Stats.DrainMana(SprintManaCost * Time.deltaTime);
				acceleration = SprintAcceleration;
				maximumSpeed = SprintMaximumSpeed;
			}
			
			// Slow when drawing the bow
			if (Stats.Buffs.Has(Buff.DrawingBow)) {
				maximumSpeed *= 0.70f;
			}

			var queuedAbility = Combat.GetQueuedAbility();
			var targetEnemy = Targeting.GetTargetedEnemy();
			var mousePoint = MouseStatus.GetWalkableWorldPoint();

			if (queuedAbility.HasValue && queuedAbility.Value.GetReason() != PlayerCombat.AbilityQueueReason.Cast) {
				SetTargetPositionToSpellTarget(queuedAbility.Value);
			} else if (CommandStatus.IsAnyActive(CommandBinding.Command.MoveToMouse, CommandBinding.Command.ForceMoveToMouse)
			           && mousePoint.HasValue && !targetEnemy.HasValue && CommandStatus.IsInactive(CommandBinding.Command.ForceCast)) {
				SetTargetPosition(mousePoint.Value);
			} else if (CommandStatus.IsAnyStoppedThisFrame(CommandBinding.Command.MoveToMouse, CommandBinding.Command.ForceMoveToMouse)) {
				ShowTargetPosition();
			}
			
			Vector3 updatedMovementDirection;
			var position = transform.position;
			if (TargetPosition.HasValue && Vector3.Distance(TargetPosition.Value, position) > 0.01f) {
				updatedMovementDirection = Vector3.Normalize(TargetPosition.Value - position);
				var decelerationModifier = Mathf.Pow(Mathf.Min(1.00f, Vector3.Distance(position, TargetPosition.Value) / DecelerationDistance), 0.5f);
				MovementSpeed += (maximumSpeed - MovementSpeed) * acceleration * Time.deltaTime;
				if (decelerationModifier >= 1.00f) {
					LastNonDeceleratingSpeed = MovementSpeed;
				} else {
					var targetSpeed = LastNonDeceleratingSpeed * decelerationModifier;
					if (MovementSpeed > targetSpeed) {
						MovementSpeed = targetSpeed;
					}
				}
				
			} else if (TargetPosition.HasValue) {
				MovementSpeed = 0f;
				updatedMovementDirection = Vector3.zero;
				MovementController.Move(TargetPosition.Value - position);
				ResetTargetPosition();
				
			} else {
				MovementSpeed = 0f;
				updatedMovementDirection = Vector3.zero;
				ResetTargetPosition();
			}
			
			MovementDirection = Vector3.Lerp(MovementDirection, updatedMovementDirection, AngularAcceleration * Time.deltaTime);
			MovementController.Move(MovementDirection * MovementSpeed * Time.deltaTime);

			var distanceToGround = Utility.GetDistanceToGround(transform.position);
			if (Mathf.Abs(ExpectedFloatingHeight - distanceToGround) > 0.01f) {
				var movementVector = new Vector3(0, ExpectedFloatingHeight - distanceToGround, 0);
				MovementController.Move(movementVector * 1.00f * Time.deltaTime);
			}

			if (IsGrounded()) {
				LastGroundedPosition = transform.position;
			}
		}
		private void Update_Blink() {
			var mousePoint = MouseStatus.GetWalkableWorldPoint();
			if (!mousePoint.HasValue) {
				return;
			}

			var targetPoint = mousePoint.Value;
			targetPoint.y += ExpectedFloatingHeight;
			
			var hasCheapBlink = Equipment.HasGlyph(Glyph.BlinkCost);
			var hasPreciseBlink = Equipment.HasGlyph(Glyph.BlinkPrecision);

			const float blinkRange = 3.00f;
			var blinkManaCost = 30.00f;
			if (hasCheapBlink) { blinkManaCost = 15.00f; }

			if (CommandStatus.IsIssuedThisFrame(CommandBinding.Command.Blink) && Stats.HasMana(blinkManaCost)) {
				Stats.DrainMana(blinkManaCost);
				Vector3 movementVector;

				if (hasPreciseBlink) {
					movementVector = Vector3.MoveTowards(Vector3.zero, targetPoint - transform.position, blinkRange);
				} else {
					movementVector = Vector3.MoveTowards(Vector3.zero, targetPoint - transform.position, 1.00f).normalized * blinkRange;
				}

				MovementController.Move(movementVector);
				ResetTargetPosition();
			}
		}

		private bool IsGrounded() {
			return Utility.GetDistanceToGround(transform.position) <= ExpectedFloatingHeight * 2.00f;
		}
		public float GetLastValidGroundHeight() {
			if (!IsGrounded()) {
				return LastGroundedPosition.y;
			}
			return transform.position.y;
		}

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			var body = hit.gameObject.GetComponent<Rigidbody>();
			if (body != null) {
				body.AddForce(hit.moveDirection * hit.moveLength * 200);
			}
		}

		private void ShowTargetPosition() {
			TargetPositionIndicator.Show();
		}

		private void SetTargetPositionToSpellTarget(QueuedPlayerAbility queuedPlayerAbility) {
			var spellTargetPosition = queuedPlayerAbility.GetTargetPosition();
			var maxDistanceDelta = 0f;
			if (queuedPlayerAbility.GetReason() == PlayerCombat.AbilityQueueReason.Range) {
				maxDistanceDelta = queuedPlayerAbility.GetAbility().GetMaximumCastRange() - 0.1f;
			}
			var moveTargetPosition = Vector3.MoveTowards(spellTargetPosition, Utility.GetGroundPosition(transform.position), maxDistanceDelta);
			SetTargetPosition(moveTargetPosition);
		}
		
		private void SetTargetPosition(Vector3 target) {
			TargetPosition = Maybe<Vector3>.Some(new Vector3(target.x, target.y + ExpectedFloatingHeight, target.z));
			TargetPositionIndicator.MoveTo(target);
			TargetPositionIndicator.Hide();
		}

		private void ResetTargetPosition() {
			TargetPosition = Maybe<Vector3>.None;
			TargetPositionIndicator.Hide();
		}

		private void OnDestroy() {
			Destroy(TargetPositionIndicator);
		}
	}
}