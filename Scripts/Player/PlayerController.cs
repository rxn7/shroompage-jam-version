using System;
using Godot;
using Game.Surface;
using Game.Utils;

namespace Game.Player;

internal class PlayerController {
	private const float CrouchHeight = 0.9f;
	private const float GravityForce = 9.9f;
	private const float JumpForce = 4.0f;
	private const float SprintVelocityThreshold = 0.1f;
	private const float AirDirectionAcceleration = 3.0f;
	private const float GroundDirectionAcceleration = 15.0f;
	private const float InAirMoveSpeed = 5.0f;
	private const float SprintMoveSpeed = 6.0f;
	private const float MoveSpeedChangeSpeed = 10.0f;
	public const float WalkMoveSpeed = 4.0f;
	private const float CrouchMoveSpeed = 2.5f;

	private static readonly StringName MoveLeftInputName = "MoveLeft";
	private static readonly StringName MoveRightInputName = "MoveRight";
	private static readonly StringName MoveForwardInputName = "MoveForward";
	private static readonly StringName MoveBackwardInputName = "MoveBackward";
	private static readonly StringName JumpInputName = "Jump";
	private static readonly StringName CrouchInputName = "Crouch";
	private static readonly StringName SprintInputName = "Sprint";

	public bool JustLanded => m_Player.IsOnFloor() && !m_WasOnFloorLastFrame;
	public float Height { get; private set; }
	public Vector3 Velocity { get; private set; }
	public bool IsMoving { get; private set; }
	public float MoveSpeed { get; private set; }
	public float TargetMoveSpeed { get; private set; }
	public EMovementState MovementState { get; private set; }
	public Vector3 Direction { get; private set; }
	public PlayerMovementInputs Inputs => m_Inputs;
	public ESurfaceMaterial SurfaceMaterial { get; private set; }

	private readonly PlayerManager m_Player;
	private readonly CollisionShape3D m_Collider;
	private readonly CapsuleShape3D m_Shape;
	private readonly float m_DefaultHeight;
	private PlayerMovementInputs m_Inputs;
	private bool m_WasOnFloorLastFrame = false;
	private bool m_IsMoving;

	public PlayerController(PlayerManager player) {
		m_Player = player;

		m_Collider = m_Player.GetNode<CollisionShape3D>("CollisionShape3D");
		m_Shape = m_Collider.Shape as CapsuleShape3D;

		m_DefaultHeight = m_Shape.Height;
	}

	public void Update(float dt) {
		UpdateInput();
		UpdateMovementState();
		UpdateDirection(dt);
		UpdateMoveSpeed(dt);
		UpdateHeight(dt);
	}

	public void PhysicsUpdate(float dt) {
		UpdatePhysicsMovement(dt);
		Velocity = m_Player.GetRealVelocity();
		m_IsMoving = Velocity.LengthSquared() >= float.Epsilon;
		
		SurfaceMaterial = m_Player.GetSurfaceMaterial();

		if (JustLanded)
			PlayFootstep();

		m_WasOnFloorLastFrame = m_Player.IsOnFloor();
	}

	public void HandleMouseMotionEvent(InputEventMouseMotion motion) {
		m_Player.Head.Pitch = Mathf.Clamp(m_Player.Head.Pitch - motion.Relative.Y * 0.1f, -90.0f, 90.0f);

		Vector3 rootRotation = m_Player.RotationDegrees + Vector3.Down * motion.Relative.X * 0.1f;
		while (rootRotation.Y >= 360.0f) rootRotation.Y -= 360.0f;
		while (rootRotation.Y <= -360.0f) rootRotation.Y += 360.0f;
		m_Player.RotationDegrees = rootRotation;
	}

	public void PlayFootstep(float volume = 0.0f) {
		SurfaceManager.PlayRandomFootstep(SurfaceMaterial, m_Player.GlobalPosition, (float)GD.RandRange(0.9f, 1.1f), volume);
	}

	private void UpdatePhysicsMovement(float dt) {
		Vector3 groundVelocity = Direction * MoveSpeed;

		float verticalVelocity = m_Player.Velocity.Y;
		if (!m_Player.IsOnFloor())
			verticalVelocity -= GravityForce * dt;
		else if (m_Inputs.Jump) {
			verticalVelocity += JumpForce;
			PlayFootstep();
		}

		m_Player.Velocity = groundVelocity + Vector3.Up * verticalVelocity;
		m_Player.MoveAndSlide();
	}

	private void UpdateInput() {
		m_Inputs.Jump = false;

		m_Inputs.Move = Input.GetVector(MoveLeftInputName, MoveRightInputName, MoveForwardInputName, MoveBackwardInputName);
		m_Inputs.Jump = Input.IsActionPressed(JumpInputName);
		m_Inputs.Crouch = Input.IsActionPressed(CrouchInputName);
		m_Inputs.Sprint = Input.IsActionPressed(SprintInputName);
	}

	private void UpdateMovementState() {
		if (!m_Player.IsOnFloor()) {
			MovementState = EMovementState.InAir; return;
		}

		if (m_Inputs.Crouch) {
			MovementState = EMovementState.Crouching;
			return;
		}

		if (!m_IsMoving) {
			MovementState = EMovementState.Idling;
			return;
		}

		if (m_Inputs.Sprint && Velocity.LengthSquared() >= SprintVelocityThreshold * SprintVelocityThreshold) {
			MovementState = EMovementState.Sprinting;
			return;
		}

		MovementState = EMovementState.Walking;
	}

	private void UpdateDirection(float dt) {
		float directionAcceleration = MovementState == EMovementState.InAir ? AirDirectionAcceleration : GroundDirectionAcceleration;

		Vector3 targetDirection = (m_Player.GlobalTransform.Basis * new Vector3(m_Inputs.Move.X, 0, m_Inputs.Move.Y)).Normalized();
		Direction = Direction.SafeLerp(targetDirection, directionAcceleration * dt);

		if (Mathf.IsZeroApprox(Direction.LengthSquared()))
			Direction = Vector3.Zero;
	}

	private void UpdateMoveSpeed(float dt) {
		TargetMoveSpeed = MovementState switch {
			EMovementState.Sprinting => SprintMoveSpeed,
			EMovementState.Crouching => CrouchMoveSpeed,
			EMovementState.InAir => InAirMoveSpeed,
			_ => WalkMoveSpeed 
		};

		MoveSpeed = Mathf.MoveToward(MoveSpeed, TargetMoveSpeed, MoveSpeedChangeSpeed * dt);
	}

	private void UpdateHeight(float dt) {
		float targetHeight = MovementState switch {
			EMovementState.Crouching => CrouchHeight,
			_ => m_DefaultHeight,
		};

		if (Height != targetHeight) {
			Height = m_Shape.Height = targetHeight;
			m_Collider.Position = Vector3.Up * Height * 0.5f;
		}
	}

	internal struct PlayerMovementInputs {
		public bool Jump { get; set; }
		public bool Crouch { get; set; }
		public bool Sprint { get; set;  }
		public Vector2 Move { get; set; }
	}
}
