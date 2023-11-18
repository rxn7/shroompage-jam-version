using System;
using Game.Player;
using Godot;

namespace Game.Enemy;

internal partial class Enemy : CharacterBody3D, IHealth {
	private const float HopCooldown = 1.0f;
	private const float HopForce = 10.0f;
	private const float HopHeight = 4.0f;

	public PlayerManager TargetPlayer { get; set; }
	public float Health { get; set; }
	public Action OnDied { get; set; }
	public bool IsDead { get; set; }

	private float m_HopCooldownTimer = 0.0f;
	private float m_HopCooldown = HopCooldown;
	private Vector3 m_Velocity;

	public Enemy() {
		MotionMode = MotionModeEnum.Grounded;
		UpDirection = Vector3.Up;
		Scale = Vector3.One * (GD.Randf() * 0.5f + 0.75f);
	}

	public override void _PhysicsProcess(double delta) {
		m_Velocity.Y -= 9.8f * (float)delta;

		if(IsOnFloor()) {
			m_Velocity.X = 0.0f;
			m_Velocity.Z = 0.0f;

			m_HopCooldownTimer += (float)delta;
			if(m_HopCooldownTimer >= HopCooldown) {
				Hop();
			}
		}

		Velocity = m_Velocity;
		MoveAndSlide();
	}

	public void Hop() {
		m_HopCooldownTimer = 0.0f;
		m_HopCooldown = HopCooldown * (GD.Randf() * 0.4f + 0.8f);

		Vector3 direction = GlobalPosition.DirectionTo(TargetPlayer.GlobalPosition);
		direction.Y = 0.0f;
		direction = direction.Normalized();

		float force = HopForce * (GD.Randf() * 0.4f + 0.8f);
		float height = HopHeight * (GD.Randf() * 0.4f + 0.8f);

		m_Velocity = direction * force + Vector3.Up * height;
	}
}
