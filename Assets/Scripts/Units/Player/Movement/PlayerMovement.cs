using Misc;
using Settings;
using UI.UserInput;
using UnityEngine;

namespace Units.Player.Movement {
	public class PlayerMovement : MonoBehaviour {
		private const float MaximumSpeed = 2.15f;
		private const float Acceleration = 5.00f;
		private const float AngularAcceleration = 10.00f;
		private const float SprintMaximumSpeed = 3.50f;
		private const float SprintAcceleration = 15.00f;
		private const float Deceleration = 10.00f;
		private const float DecelerationDistance = 0.50f;
		private const float SprintManaCost = 5.00f;
		private const float SprintManaBuffer = 5.00f;

		private float MovementSpeed;
		private Vector3 MovementDirection;
		private Vector3? TargetPosition;
		private bool IsSprinting;

		private readonly Timer FloatingTimer = new Timer();
		private const float ExpectedFloatingHeight = 0.6f;
		private Vector3 LastGroundedPosition;

		private UnitStats Stats;
		private PlayerEquipment Equipment;
		private CharacterController MovementController;
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();
		private readonly MouseStatus MouseStatus = AutowireFactory.GetInstanceOf<MouseStatus>();

		private void Start() {
			Stats = GetComponent<UnitStats>();
			Equipment = GetComponent<PlayerEquipment>();
			MovementController = GetComponent<CharacterController>();
			FloatingTimer.StartForever(1.00f);
		}

		private void Update() {
			Update_Movement();
			Update_Blink();
		}
		private void Update_Movement() {
			if (Stats.IsDead()) { return; }

			var isSprintKeyDown = CommandStatus.IsActive(CommandBinding.Command.Sprint);
			if (isSprintKeyDown && TargetPosition != null && Stats.HasMana(SprintManaBuffer)) {
				IsSprinting = true;
			} else if (!isSprintKeyDown || TargetPosition == null || !Stats.HasMana(SprintManaCost * Time.deltaTime)) {
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
			if (Stats.buffs.Has(Buff.DrawingBow)) {
				maximumSpeed *= 0.70f;
			}

			Vector3 updatedMovementDirection;
			var decelerationModifier = 1.00f;
			var mousePoint = MouseStatus.GetWorldPointForWalkableLayer();
			if (CommandStatus.IsActive(CommandBinding.Command.MoveToMouse) && mousePoint.HasValue) {
				TargetPosition = new Vector3(mousePoint.Value.x, mousePoint.Value.y + ExpectedFloatingHeight, mousePoint.Value.z);
			}
			
			if (TargetPosition != null) {
				updatedMovementDirection = Vector3.Normalize(TargetPosition.Value - transform.position);
				decelerationModifier = Mathf.Min(1.00f, Vector3.Distance(transform.position, TargetPosition.Value) / DecelerationDistance);
				if (Vector3.Distance(TargetPosition.Value, MovementController.transform.position) < 0.01f) {
					TargetPosition = null;
				}
			} else {
				updatedMovementDirection = Vector3.zero;
			}
			
			MovementSpeed += (maximumSpeed - MovementSpeed) * acceleration * Time.deltaTime;
			MovementSpeed -= MovementSpeed * (1.0f - Mathf.Pow(decelerationModifier, 1f / Deceleration));
			MovementDirection = Vector3.Lerp(MovementDirection, updatedMovementDirection, AngularAcceleration * Time.deltaTime);
			MovementController.Move(MovementDirection * MovementSpeed * Time.deltaTime);

			var distanceToGround = GetControllerDistanceToGround();
			if (Mathf.Abs(ExpectedFloatingHeight - distanceToGround) > 0.01f) {
				var movementVector = new Vector3(0, ExpectedFloatingHeight - distanceToGround, 0);
				MovementController.Move(movementVector * 1.00f * Time.deltaTime);
			}

			if (IsGrounded()) {
				LastGroundedPosition = transform.position;
			}
		}
		private void Update_Blink() {
			var mousePoint = MouseStatus.GetWorldPointForWalkableLayer();
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

			if (CommandStatus.IsIssued(CommandBinding.Command.Blink) && Stats.HasMana(blinkManaCost)) {
				Stats.DrainMana(blinkManaCost);
				Vector3 movementVector;

				if (hasPreciseBlink) {
					movementVector = Vector3.MoveTowards(Vector3.zero, targetPoint - transform.position, blinkRange);
				} else {
					movementVector = Vector3.MoveTowards(Vector3.zero, targetPoint - transform.position, 1.00f).normalized * blinkRange;
				}

				MovementController.Move(movementVector);
				TargetPosition = null;
			}
		}

		private bool IsGrounded() {
			var distanceToGround = GetControllerDistanceToGround();
			return distanceToGround <= ExpectedFloatingHeight * 2.00f;
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

		private float GetControllerDistanceToGround() {
			RaycastHit hit;
			const int walkableLayerMask = 1 << 9;
			var ray = new Ray(transform.position, Vector3.down);

			if (Physics.Raycast(ray, out hit, 1000.00f, walkableLayerMask)) {
				return hit.distance;
			}

			return 1000.00f;
		}
	}
}