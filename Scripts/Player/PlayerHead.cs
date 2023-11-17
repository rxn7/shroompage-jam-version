using Godot;
using Game.Utils;

namespace Game.Player;

internal partial class PlayerHead : Node3D {
	private const float YOffset = 0.2f;
	private const float BobAmplitude = 0.05f;
	private const float BobRollAmplitude = 0.2f;
	private const float StrafeRollMaxAngleDegrees = 6.0f;
	private const float StrafeRollLerpSpeed = 6.0f;
	private const float HeightChangeSpeed = 8.0f;

	public float Pitch { get; set;}
	public float StrafeRoll { get; private set; }
	public Camera3D Camera { get; private set; }

	[Export] private PlayerManager m_Player;
	private Transform3D m_OldTransform, m_Transform; 
	private bool m_UpdateTransform;
	private float m_Height;

	public override void _Ready() {
		TopLevel = true;

		GlobalTransform = m_Player.GlobalTransform;
		m_OldTransform = GlobalTransform;
		m_Transform = GlobalTransform;

		Camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _Process(double dt) {
		float fdt = (float)dt;
		m_Height = MathUtils.SafeLerp(m_Height, m_Player.Controller.Height, HeightChangeSpeed * fdt);
		SmoothFollowPlayer(fdt);
		StrafeRoll = MathUtils.SafeLerp(StrafeRoll, CalculateStrafeRoll(), StrafeRollLerpSpeed * fdt);
		UpdateCameraTransform(fdt);
	}

	public override void _PhysicsProcess(double dt) {
		m_UpdateTransform = true;
	}

	private float CalculateStrafeRoll() {
		float strafe = m_Player.GetRealVelocity().Dot(-GlobalTransform.Basis.X) * 0.3f;
		return Mathf.Clamp(strafe, -StrafeRollMaxAngleDegrees, StrafeRollMaxAngleDegrees);
	}

	private void SmoothFollowPlayer(float dt) {
		if(m_UpdateTransform) {
			m_UpdateTransform = false;
			m_OldTransform = m_Transform;
			m_Transform = m_Player.GlobalTransform;
		}

		float f = Mathf.Clamp((float)Engine.GetPhysicsInterpolationFraction(), 0.0f, 1.0f);

		GlobalTransform = m_OldTransform.InterpolateWith(m_Transform, f).TranslatedLocal(Vector3.Up * (m_Height - YOffset)).RotatedLocal(Vector3.Right, Mathf.DegToRad(Pitch));
	}

	private void UpdateCameraTransform(float dt) {
		Camera.Position = new Vector3(m_Player.Bobbing.Bob.X, m_Player.Bobbing.Bob.Y, 0.0f) * BobAmplitude;
		Camera.RotationDegrees = Vector3.Back * (StrafeRoll + m_Player.Bobbing.Bob.Y * BobRollAmplitude);
	}
}
