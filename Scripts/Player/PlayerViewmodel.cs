using Godot;
using Game.Utils;

namespace Game.Player;

internal partial class PlayerViewmodel : Node3D {
	private const float BobLerpSpeed = 15f;
	private const float BobAmplitude = 0.005f;
	private const float SwayAmount = 5f;
	private const float MaxSwayAmount = 30f;
	private const float SwayLerpSpeed = 10.0f;
	private const float SwayResetMouseLastMovedTimerThreshold = 0.05f;
	private const float HeadRollMultiplier = 3.0f;
	private const float JumpFeedbackOffset = 0.001f;

	public Vector3 Sway { get; private set; }
	public AnimationPlayer AnimPlayer { get; private set; }
	public PlayerManager Player { get; set; }

	[Export] private Node3D m_LeftHandSlot, m_RightHandSlot;

	private Vector2 m_LastCameraRotationDegrees;
	private Vector2 m_TargetSway;
	private Vector3 m_Bob;
	private Vector3 m_InitPosition;
	private Vector2 m_CameraRotationDelta; 
	private float m_MouseLastMovedTimer = 0.0f; 

	public override void _Ready() {
		m_InitPosition = Position;
		AnimPlayer = GetNode<AnimationPlayer>("Viewmodel/AnimationPlayer");
		AnimPlayer.AnimationFinished += AnimationFinished;
		PlayEquipAnimation();
	}

	public override void _ExitTree() {
		AnimPlayer.AnimationFinished -= AnimationFinished;
	}

	public override void _Input(InputEvent e) {
		if(e is InputEventMouseMotion m) {
			Vector2 cameraRotationDelta = new Vector2(
				MathUtils.AngleDelta(m_LastCameraRotationDegrees.Y, Player.Head.CameraRotationDegrees.Y),
				-MathUtils.AngleDelta(m_LastCameraRotationDegrees.X, Player.Head.CameraRotationDegrees.X)
			);
			m_LastCameraRotationDegrees = Player.Head.CameraRotationDegrees;

			m_TargetSway = cameraRotationDelta * SwayAmount;
			m_TargetSway.X = Mathf.Clamp(m_TargetSway.X, -MaxSwayAmount, MaxSwayAmount);
			m_TargetSway.Y = Mathf.Clamp(m_TargetSway.Y, -MaxSwayAmount, MaxSwayAmount);

			m_MouseLastMovedTimer = 0.0f;
		}
	}

	public override void _Process(double dt) {
		m_MouseLastMovedTimer += (float)dt;
		if(m_MouseLastMovedTimer >= SwayResetMouseLastMovedTimerThreshold)
			m_TargetSway = Vector2.Zero;

		Sway = Sway.SafeLerp(new Vector3(m_TargetSway.X, m_TargetSway.Y, 0.0f), SwayLerpSpeed * (float)dt);
		m_Bob = m_Bob.SafeLerp(new Vector3(Player.Bobbing.Bob.X, Player.Bobbing.Bob.Y, 0.0f) * BobAmplitude, BobLerpSpeed * (float)dt);

		if(!Player.IsOnFloor())
			m_Bob += Vector3.Up * JumpFeedbackOffset;

		Position = m_InitPosition + m_Bob;
		RotationDegrees = new Vector3(Sway.Y, -Sway.X, Player.Head.StrafeRoll * HeadRollMultiplier);
	}

	public void AttachToHandSlot(Node3D node, Vector3 offset = default, Vector3 rotation = default, bool right = true) {
		node.GetParent()?.RemoveChild(node);
		Node3D slot = right ? m_RightHandSlot : m_LeftHandSlot;
		slot.AddChild(node);
		node.Position = offset;
		node.RotationDegrees = rotation;
	}

	public void PlayIdleAnimation() => AnimPlayer.Play(Player?.ItemManager.HeldItem?.HoldableData.IdleAnimationName ?? "Idle");
	public void PlayEquipAnimation() => AnimPlayer.Play(Player?.ItemManager.HeldItem?.HoldableData.EquipAnimationName ?? "Equip");
	public void PlayAttackAnimation() => AnimPlayer.Play(Player?.ItemManager.HeldItem?.HoldableData.AttackAnimationName ?? "Attack");

	private void AnimationFinished(StringName name) {
		PlayIdleAnimation();
	}
}
