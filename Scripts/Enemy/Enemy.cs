using System;
using Game.ItemSystem;
using Game.Utils;
using Godot;

namespace Game.Enemy;

internal partial class Enemy : CharacterBody3D, IHealth {
	private static readonly Color DamageEffectColor = new Color(4.0f, 0.0f, 0.0f);
	private static readonly StandardMaterial3D DamageEffectMaterial = new StandardMaterial3D() { AlbedoColor = DamageEffectColor, Roughness = 1.0f, Metallic = 0.0f };
	private static readonly ItemData[] m_DeathDropItems = new ItemData[] {ItemSpawner.BatteryMushroomData, ItemSpawner.MagicMushroomData};  

	private const float DamageEffectDuration = 0.1f;
	private const float DamageCooldown = 1.0f;

	[ExportSubgroup("Stats")]
	[Export] private float HopCooldown = 1.0f;
	[Export] private float HopForce = 10.0f;
	[Export] private float HopHeight = 4.0f;
	[Export] public float Damage { get; private set; } = 5.0f;
	[Export] public float HighLevelIncrease { get; private set; } = 0.075f;
	[Export] public float DamagePlayerMaxDistanceSquared { get; private set; } = 1.2f; // Distance at which the player gets hurt

	public float Health { get; set; } = 30.0f;
	public Action OnDied { get; set; }
	public bool IsDead { get; set; }
	public Action<float> OnDamage { get; set; }

	private MeshInstance3D m_MeshInstance;
	private float m_DamageCooldownTimer = 0.0f;
	private float m_HopCooldownTimer = 0.0f;
	private float m_HopCooldown;
	private float m_DamageEffectTimer = 0.0f;
	private Vector3 m_Velocity;
	private Vector3 m_Force;
	private bool m_WasOnFloor = false;

	public Enemy() {
		MotionMode = MotionModeEnum.Grounded;
		UpDirection = Vector3.Up;
		m_HopCooldown = HopCooldown * (GD.Randf() * 0.4f + 0.8f);
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
			m_DamageEffectTimer = DamageEffectDuration;
			SpawnDamageParticles();
		};
	}

	public override void _Ready() {
		m_MeshInstance = GetChild<MeshInstance3D>(0);
	}

	public override void _Process(double delta) {
		UpdateDamageEffect((float)delta);
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
			(GameManager.Singleton.Player as IHealth).Damage(Damage);
			GameManager.Singleton.Player.HighLevel += HighLevelIncrease;
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

	private void UpdateDamageEffect(float delta) {
		if(m_DamageEffectTimer > 0.0f) {
			m_DamageEffectTimer -= delta;

			m_MeshInstance.MaterialOverride = DamageEffectMaterial;
			return;
		}

		m_MeshInstance.MaterialOverride = null;
	}

	private void SpawnDamageParticles() {
		ParticleFx deathParticleFx = ParticleFx.s_DeathParticles.Instantiate<ParticleFx>();
		GetTree().Root.AddChild(deathParticleFx);
		deathParticleFx.GlobalPosition = GlobalPosition + Vector3.Up;
		deathParticleFx.Emitting = true;
	}
}
