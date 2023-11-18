using System.Collections.Generic;
using Game.Player;
using Godot;

namespace Game.Enemy.Stage;

internal partial class Stage : Area3D {
	private List<EnemySpawner> m_Spawners = new();
	
	public Stage() {
		BodyEntered += OnBodyEntered;
	}

	public override void _Ready() {
		foreach(Node child in GetChildren()) {
			if(child is EnemySpawner spawner) {
				spawner.Stage = this;
				m_Spawners.Add(spawner);
			}
		}

	}
	public override void _ExitTree() {
		BodyEntered -= OnBodyEntered;
	}

	private void OnBodyEntered(Node body) {
		if(body is PlayerManager player) {
			Start();
		}
	}

	public void OnEnemyDied(EnemySpawner spawner) {
		m_Spawners.Remove(spawner);

		GD.Print($"Enemy has been killed, {m_Spawners.Count} left");

		if(m_Spawners.Count == 0)
			StageCleared();
	}

	private void Start() {
		GameManager.Singleton.StageStarted?.Invoke(this);
		GD.Print($"Starting stage {this} with {m_Spawners.Count} spawners.");
		foreach(EnemySpawner spawner in m_Spawners)
			spawner.Start();
	}

	private void StageCleared() {
		GameManager.Singleton.StageCleared?.Invoke(this);
		GD.Print($"Stage {this} has been cleared.");
		QueueFree();
	}
}
