using System.Collections.Generic;
using Game.Player;
using Game.Utils;
using Godot;

namespace Game.Enemy.Stage;

internal partial class Stage : Area3D {
	private static readonly AudioStream s_StageStartAudio = GD.Load<AudioStream>("res://Audio/stage_start_stinger.wav");

	private bool m_Started = false;
	private List<EnemySpawner> m_Spawners = new();

	public int EnemyCount => m_Spawners.Count;
	[Export] private bool m_IncreaseOstStageAfterClear = false;
	

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
		if(m_Started)
			return;

		if(body is PlayerManager) {
			Start();
		}
	}

	public void OnEnemyDied(EnemySpawner spawner) {
		m_Spawners.Remove(spawner);
		GD.Print($"Enemy has been killed, {m_Spawners.Count} left");

		if(EnemyCount == 0)
			StageCleared();
	}

	private void Start() {
		GameManager.Singleton.StageStarted?.Invoke(this);
		GD.Print($"Starting stage {this} with {m_Spawners.Count} spawners.");
		foreach(EnemySpawner spawner in m_Spawners)
			spawner.Start();

		m_Started = true;

		SoundManager.Play3D(GlobalPosition, s_StageStartAudio);
	}

	private void StageCleared() {
		GameManager.Singleton.StageCleared?.Invoke(this);
		GD.Print($"Stage {this} has been cleared.");

		// if (m_IncreaseOstStageAfterClear) GameManager.Singleton.Soundtrack.IncreaseStage();

		QueueFree();
	}
}
