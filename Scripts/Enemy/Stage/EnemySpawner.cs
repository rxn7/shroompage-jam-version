using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Utils;
using Godot;

namespace Game.Enemy.Stage;

internal partial class EnemySpawner : Node3D {
	public Stage Stage { get; set; }

	private static readonly List<AudioStream> s_SpawnSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/EnemySpawn");
	[Export] private int m_SpawnDelayMs;
	[Export] private PackedScene m_EnemyScene;

	public async void Start() {
		await Task.Delay(m_SpawnDelayMs);
		SpawnEnemy();
	}

	public void SpawnEnemy() {
		Enemy enemy = m_EnemyScene.Instantiate<Enemy>();
		enemy.OnDied += () => Stage.OnEnemyDied(this);

		AddChild(enemy);
		enemy.GlobalPosition = GlobalPosition;

		SoundManager.Play3D(GlobalPosition, s_SpawnSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f));

		ParticleFx spawnParticleFx = ParticleFx.s_SpawnParticles.Instantiate<ParticleFx>();
		GetTree().Root.AddChild(spawnParticleFx);
		spawnParticleFx.GlobalPosition = GlobalPosition;
		spawnParticleFx.Emitting = true;
	}
}
