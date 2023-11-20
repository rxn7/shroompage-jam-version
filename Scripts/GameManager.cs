
using System;
using Game.Enemy.Stage;
using Game.Player;
using Game.Story;
using Godot;

namespace Game;

internal partial class GameManager : Node {
	public static GameManager Singleton { get; private set; }

	public PlayerManager Player { get; private set; }
	public GameSoundtrack Soundtrack { get; private set; }
	public Stage CurrentStage { get; set; }
	public StoryIntro StoryIntro { get; private set; }

	public Action<Stage> StageStarted;
	public Action<Stage> StageCleared;

	public GameManager() {
		Singleton = this;
		StageStarted += (Stage stage) => CurrentStage = stage;
		StageCleared += OnStageCleared;
	}

	public override void _EnterTree() {
		Soundtrack = GetNode<GameSoundtrack>("Soundtrack");
		Player = GetNode<PlayerManager>("Player");
	}

	public override void _Ready() {
		// not all scenes will have an intro
		// this can be generalized to run a single scene specific script but idc since this is a jam
		StoryIntro intro = GetNodeOrNull<StoryIntro>("Intro");
		
		if (intro != null) {
			StoryIntro = intro;
			intro.Start(this);
		}
	}

	public int GetEnemyCount() {
		if(CurrentStage is null) {
			return 0;
		}

		return CurrentStage.EnemyCount;
	}

	private void OnStageCleared(Stage stage) {
		CurrentStage = null;

		if(stage.Name == "Stage1") {
			GameManager.Singleton.Player.NotificationDisplay.DisplayNotification("Stop the big shroom from blooming!", 5.0f);
		} else if(stage.Name == "Stage5") {
			GameManager.Singleton.Player.NotificationDisplay.DisplayNotification("Burn down the shroom", 5.0f);
		} else if(stage.Name == "Stage6") {
			GameEnd();
		}
	}

	private void GameEnd() {
		GameManager.Singleton.Player.HighLevel = 0.0f;
		GameManager.Singleton.Player.Controller.Locked = true;
		GameManager.Singleton.Player.BlackoutFrame.Visible = true;
		Soundtrack.SetMuted(true);
		AudioStreamPlayer endingSoundPlayer = GetNode<AudioStreamPlayer>("EndGameSoundPlayer");
		endingSoundPlayer.Play();
		endingSoundPlayer.Finished += () => {
			GetTree().ChangeSceneToFile("res://Scenes/UI/Ending.tscn");
		};
	}
}
