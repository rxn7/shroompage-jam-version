using System;
using System.Threading.Tasks;
using Godot;

namespace Game.SplashScreen;

internal partial class SplashScreen : Node {
	private SplashScreenStage[] m_Stages;
	private int m_CurrentStageIdx = 0;

	private SplashScreenStage CurrentStage => m_Stages[m_CurrentStageIdx];

	public override async void _Ready() {
		Node stagesContainer = GetNode<Control>("Stages");

		m_Stages = new SplashScreenStage[stagesContainer.GetChildCount()];
		for(int i=0; i<m_Stages.Length; ++i) {
			m_Stages[i] = stagesContainer.GetChild<SplashScreenStage>(i);
			m_Stages[i].Visible = false;
		}

		await PlayCurrentStage();
	}

	public override void _Input(InputEvent ev) {
		if((ev is not InputEventKey && ev is not InputEventMouseButton) || ev.IsEcho() || !ev.IsPressed())
			return;

		NextStage();
	}

	private async void NextStage() {
		CurrentStage.Visible = false;

		if(m_CurrentStageIdx == m_Stages.Length-1) {
			End();
			return;
		}

		m_CurrentStageIdx++;
		await PlayCurrentStage();
	}

	private void End() {
		// TODO: Main menu
		GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	}

	private async Task PlayCurrentStage() {
		await FadeIn();
		await Task.Delay(TimeSpan.FromSeconds(CurrentStage.DisplayTime));
		await FadeOut();
	}

	private async Task Fade(bool fadeOut) {
		if(!SplashScreenStage.IsInstanceValid(CurrentStage))
			return;
		
		CurrentStage.SelfModulate = new Color(1.0f, 1.0f, 1.0f, fadeOut ? 1.0f : 0.0f);

		float timer = 0.0f;
		while(timer <= CurrentStage.FadeTime) {
			float opacity = fadeOut ? 1.0f - timer / CurrentStage.FadeTime : timer / CurrentStage.FadeTime;
			CurrentStage.Modulate = new Color(1.0f, 1.0f, 1.0f, opacity);

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			timer += (float)GetProcessDeltaTime();
		}

		CurrentStage.SelfModulate = new Color(1.0f, 1.0f, 1.0f, fadeOut ? 0.0f : 0.0f);
	}

	private async Task FadeIn() {
		if(!SplashScreenStage.IsInstanceValid(CurrentStage))
			return;

		CurrentStage.Visible = true;
		await Fade(false);
	}

	private async Task FadeOut() {
		await Fade(true);

		if(!SplashScreenStage.IsInstanceValid(CurrentStage))
			return;

		CurrentStage.Visible = false;
		NextStage();
	}
}
