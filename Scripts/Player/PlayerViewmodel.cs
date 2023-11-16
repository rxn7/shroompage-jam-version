using Godot;
using Game.Player;
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

	public Vector3 Sway { get; private set; }
	[Export] public AnimationPlayer AnimPlayer { get; private set; }

	[Export] private PlayerManager m_Player;
	[Export] private Node3D m_LeftHandSlot, m_RightHandSlot;
	private Vector2 m_LastCameraRotationDegrees;
	private Vector2 m_TargetSway;
	private Vector3 m_Bob;
	private Vector3 m_InitPosition;
	private Vector2 m_CameraRotationDelta; 
	private float m_MouseLastMovedTimer = 0.0f; 

	public override void _Ready() {
		m_InitPosition = Position;
		AnimPlayer = GetNode<AnimationPlayer>("Viewmodel2/AnimationPlayer");
		AnimPlayer.AnimationFinished += AnimationFinished;
		PlayAnimation("Equip");
	}

	public override void _Input(InputEvent e) {
		if(e is InputEventMouseMotion m) {
			Vector2 cameraRotationDelta = new Vector2(
				MathUtils.AngleDelta(m_LastCameraRotationDegrees.Y, m_Player.Head.CameraRotationDegrees.Y),
				-MathUtils.AngleDelta(m_LastCameraRotationDegrees.X, m_Player.Head.CameraRotationDegrees.X)
			);
			m_LastCameraRotationDegrees = m_Player.Head.CameraRotationDegrees;

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
		m_Bob = m_Bob.SafeLerp(new Vector3(m_Player.Bobbing.Bob.X, m_Player.Bobbing.Bob.Y, 0.0f) * BobAmplitude, BobLerpSpeed * (float)dt);

		Position = m_InitPosition + m_Bob;
		RotationDegrees = new Vector3(Sway.Y, -Sway.X, m_Player.Head.StrafeRoll * HeadRollMultiplier);
	}

	public void PlayAnimation(StringName animation) {
		AnimPlayer.Play(animation);
	}

	public void AttachToHandSlot(Node3D node, Vector3 offset = default, Vector3 rotation = default, bool right = true) {
		node.GetParent()?.RemoveChild(node);
		Node3D slot = right ? m_RightHandSlot : m_LeftHandSlot;
		slot.AddChild(node);
		node.Position = offset;
		node.RotationDegrees = rotation;
	}

	private void AnimationFinished(StringName name) {
		AnimPlayer.Play("Idle");
	}
}
