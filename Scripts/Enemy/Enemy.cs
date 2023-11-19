using System;
using Game.ItemSystem;
using Game.Utils;
using Godot;

namespace Game.Enemy;

internal partial class Enemy : CharacterBody3D, IHealth {
	private const float DamagePlayerMaxDistanceSquared = 1.2f;
	private const float HopCooldown = 1.0f;
	private const float HopForce = 10.0f;
	private const float HopHeight = 4.0f;
	private const float DamageCooldown = 1.0f;

	public float Health { get; set; } = 30.0f;
	public Action OnDied { get; set; }
	public bool IsDead { get; set; }
	public Action<float> OnDamage { get; set; }

	[Export] private float m_Damage = 5.0f;
	[Export] private float m_HighLevelIncrease = 0.075f;
	[Export] private ItemData[] m_DeathDropItems = new ItemData[] {ItemSpawner.BatteryMushroomData, ItemSpawner.MagicMushroomData};  
	private float m_DamageCooldownTimer = 0.0f;
	private float m_HopCooldownTimer = 0.0f;
	private float m_HopCooldown = HopCooldown;
	private Vector3 m_Velocity;
	private Vector3 m_Force;
	private bool m_WasOnFloor = false;

	public Enemy() {
		MotionMode = MotionModeEnum.Grounded;
		UpDirection = Vector3.Up;
		FloorStopOnSlope = true;
		Scale = Vector3.One * (GD.Randf() * 0.5f + 0.75f);
		RotationDegrees += Vector3.Up * GD.Randf() * 360.0f;

		OnDied += () => {
			Item dropItem = m_DeathDropItems.GetRandomItem().Spawn();
			GetTree().CurrentScene.AddChild(dropItem);
			dropItem.GlobalPosition = GlobalPosition + Vector3.Up;

			QueueFree();
		};

		OnDamage += (float dmg) => {
			// TODO: PLAY THEM PARTICLES
		};
	}

	public override void _PhysicsProcess(double delta) {
		if(IsOnFloor()) { 
			m_Velocity = Vector3.Zero;

			m_HopCooldownTimer += (float)delta;
			if(m_HopCooldownTimer >= HopCooldown)
				Hop();
		} else {
			m_Velocity.Y -= 9.8f * (float)delta;
		}

		m_Velocity += m_Force;
		m_Force = Vector3.Zero;

		Velocity = m_Velocity;
		MoveAndSlide();

		if(m_DamageCooldownTimer < DamageCooldown) {
			m_DamageCooldownTimer += (float)delta;
		} else if(GlobalPosition.DistanceSquaredTo(GameManager.Singleton.Player.GlobalPosition) <= DamagePlayerMaxDistanceSquared) {
			(GameManager.Singleton.Player as IHealth).Damage(m_Damage);
			GameManager.Singleton.Player.HighLevel += m_HighLevelIncrease;
			m_DamageCooldownTimer = 0.0f;
		}
	}

	public void ApplyImpulse(Vector3 force) {
		m_Force += force;
	}

	public void Hop() {
		m_DamageCooldownTimer = DamageCooldown;
		m_HopCooldownTimer = 0.0f;
		m_HopCooldown = HopCooldown * (GD.Randf() * 0.4f + 0.8f);

		Vector3 direction = GlobalPosition.DirectionTo(GameManager.Singleton.Player.GlobalPosition);
		direction.Y = 0.0f;
		direction = direction.Normalized();

		float force = HopForce * (GD.Randf() * 0.4f + 0.8f);
		float height = HopHeight * (GD.Randf() * 0.4f + 0.8f);

		m_Velocity = direction * force + Vector3.Up * height;
	}
}
