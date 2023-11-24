
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
		StoryIntro intro = GetNodeOrNull<StoryIntro>("Intro");
		if (intro != null) {
			StoryIntro = intro;
			intro.Start(this);
		}

		intro.OnFinish += () => {
			SpeedrunData.SpeedrunStartMsec = Time.GetTicksMsec();
		};
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
			Player.NotificationDisplay.DisplayNotification("Stop the big shroom from blooming!", 5.0f);
		} else if(stage.Name == "Stage5") {
			Player.NotificationDisplay.DisplayNotification("Burn down the shroom", 5.0f);
		} else if(stage.Name == "Stage6") {
			GameEnd();
		}
	}

	private void GameEnd() {
		Player.HighLevel = 0.0f;
		Player.Controller.Locked = true;
		Player.BlackoutFrame.Visible = true;
		Soundtrack.SetMuted(true);

		SpeedrunData.SpeedrunEndMsec = Time.GetTicksMsec();

		AudioStreamPlayer endingSoundPlayer = GetNode<AudioStreamPlayer>("EndGameSoundPlayer");
		endingSoundPlayer.Play();
		endingSoundPlayer.Finished += () => {
			GetTree().ChangeSceneToFile("res://Scenes/UI/Ending.tscn");
		};
	}
}
