using System.Threading.Tasks;
using Godot;

namespace Game.Enemy.Stage;

internal partial class EnemySpawner : Node3D {
	public Stage Stage { get; set; }

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
	}
}
