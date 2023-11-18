using System;
using Game.Enemy.Stage;
using Game.Player;
using Godot;

namespace Game;

internal partial class GameManager : Node {
	public static GameManager Singleton { get; private set; }

	[Export] public PlayerManager Player { get; private set; }

	public Action<Stage> StageStarted;
	public Action<Stage> StageCleared;

	public GameManager() {
		Singleton = this;
	}
}
