using System;
using Game.Enemy.Stage;
using Game.Player;
using Godot;

namespace Game;

internal partial class GameManager : Node {
	public static GameManager Singleton { get; private set; }

	[Export] public PlayerManager Player { get; private set; }
	public Stage CurrentStage { get; set; }

	public Action<Stage> StageStarted;
	public Action<Stage> StageCleared;

	public GameManager() {
		Singleton = this;
		StageStarted += (Stage stage) => CurrentStage = stage;
		StageCleared += (Stage stage) => CurrentStage = null;
	}

	public int GetEnemyCount() {
		if(CurrentStage is null) {
			return 0;
		}

		return CurrentStage.EnemyCount;
	}
}
