using Godot;
using Game.Player;
using Game.Utils;

namespace Game.Player;

internal class PlayerBobbing {
	private const float BobAmplitude = 1.0f;
	private const float BobResetLerpSpeed = 5.0f;
	private const float BobVelocityThreshold = 0.1f;

	public Vector2 Bob { get; private set; }

	private readonly PlayerManager m_Player;
	private readonly SineWaveBase m_BobWaveBaseX = new SineWaveBase();
	private readonly CosineWaveBase m_BobWaveBaseY = new CosineWaveBase();
	private Vector2 m_LastBob;

	public PlayerBobbing(PlayerManager player) {
		m_Player = player;
		m_BobWaveBaseY.Offset = MathUtils.PiHalf;
	}

	public void Update(float dt) {
		if (m_Player.Controller.MovementState == EMovementState.InAir || m_Player.Controller.Inputs.Move.LengthSquared() == 0.0f || m_Player.Controller.Velocity .LengthSquared() < BobVelocityThreshold * BobVelocityThreshold) {
			Bob = Bob.SafeLerp(Vector2.Zero, BobResetLerpSpeed * dt);
			m_BobWaveBaseX.Reset();
			m_BobWaveBaseY.Reset();
			return;
		}

		const float stepsPerMeter = 1.5f;
		float footstepFrequency = m_Player.Controller.MoveSpeed / stepsPerMeter * Mathf.Pi * 2.0f;
		float moveSpeedRatio = m_Player.Controller.MoveSpeed / PlayerController.WalkMoveSpeed;

		Bob = new Vector2(
			m_BobWaveBaseX.GetValue(footstepFrequency * 0.5f), 
			m_BobWaveBaseY.GetValue(footstepFrequency)
		) * moveSpeedRatio;

		m_BobWaveBaseX.UpdateTimer(dt);
		m_BobWaveBaseY.UpdateTimer(dt);

		if (Bob.Y < 0 && m_LastBob.Y >= 0) {
			float volume = 20.0f * Mathf.Log(moveSpeedRatio) - 20.0f;
			m_Player.Controller.PlayFootstep(volume);
		}

		m_LastBob = Bob;
	}
}
