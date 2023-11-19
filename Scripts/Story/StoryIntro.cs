

using System;
using System.Threading.Tasks;
using Game.Player;
using Game.Utils;
using Godot;

namespace Game.Story;

// TODO this should really inherit from a base StoryElement class since it's referenced a lot
internal partial class StoryIntro : Node {
	[Export] private bool m_DebugDisableIntro = false;
	public bool DisableShroomEffects = true;

	private Barrier m_IntroBarrier;
	private PlayerNotificationDisplay m_NotificationDisplay;
	private GameSoundtrack m_Soundtrack;    
	private PlayerManager m_Player;
	private Label m_ShroomCollectProgress;
	private AudioStreamPlayer m_EndSequenceSoundPlayer;

	private bool m_FinishedTextIntro = false;
	private bool m_FinishedIntro = false;

	private int m_CurrentMessage = 0;
	private double m_MessageTimer = 3;
	private int m_collectedShrooms = 0;
	private bool m_playedLastShroomNotification = false;
	private readonly String[] m_Messages = {
		"Press [WASD] to move",
		"Press [Space] to jump",
		"Press [Left Shift] to sprint",
		"Harvest 6 mushrooms",
	};

	public async Task Start(GameManager game) {
		m_Player = game.Player;
		m_NotificationDisplay = game.Player.NotificationDisplay;
		m_Soundtrack = game.Soundtrack;
   		m_ShroomCollectProgress = game.Player.HUD.GetNode<Label>("ShroomProgress");
		m_IntroBarrier = GetNode<Barrier>("IntroGate");
		m_EndSequenceSoundPlayer = GetNode<AudioStreamPlayer>("EndSequenceSoundPlayer");

		m_Player.ViewmodelDisabled = true;
		m_Soundtrack.SetIntroMusic(true);

		// it makes a loud sound :(
		await Task.Delay(600).ContinueWith(t => {
			m_Soundtrack.SetMuted(false);
		});

		if (m_DebugDisableIntro) {
			m_NotificationDisplay.DisplayNotification("INTRO DISABLED", 3);
			m_Soundtrack.SetIntroMusic(false);
			m_Player.GlobalPosition = m_IntroBarrier.GlobalPosition;
			m_IntroBarrier.QueueFree();
			m_playedLastShroomNotification = true;
			m_FinishedIntro = true;
			m_Player.ViewmodelDisabled = false;
			DisableShroomEffects = false;
			m_collectedShrooms = 100;
		}
	}

	public override void _Process(double delta_time) {
		if (m_FinishedIntro) {
			m_ShroomCollectProgress.Text = "";
			return;
		} 

		m_ShroomCollectProgress.Text = $"Mushrooms: {m_collectedShrooms}/6";
		TextIntroUpdate(delta_time);
	}

	private void TextIntroUpdate(double delta_time) {
		m_MessageTimer -= delta_time;

		if (m_playedLastShroomNotification) return;
		if (m_FinishedTextIntro) return;
		if (m_MessageTimer > 0) return;

		m_MessageTimer = 4;
		m_NotificationDisplay.DisplayNotification(m_Messages[m_CurrentMessage], 5);
		m_CurrentMessage += 1;

		if (m_CurrentMessage >= m_Messages.Length) {
			FinishedIntroText();
			return;
		}
	}

	private void FinishedIntroText() {
		m_FinishedTextIntro = true;
		m_ShroomCollectProgress.Show();
	}

	public async void CollectShroom() {
		m_collectedShrooms++;
		
		if (m_collectedShrooms == 6) {
			await BeginEndSequence();
			return;
		}

		if (m_collectedShrooms != 5) 
			return;

		m_playedLastShroomNotification = true;
		m_NotificationDisplay.DisplayNotification("I should eat one of these", 3);
	}

	private async Task BeginEndSequence() {
		m_EndSequenceSoundPlayer.Play();

		GameManager.Singleton.Player.BlackoutFrame.Visible = true;

		GameManager.Singleton.Soundtrack.SetMuted(true);

		await Task.Delay(5000);

		GameManager.Singleton.Soundtrack.SetIntroMusic(false);
		GameManager.Singleton.Player.BlackoutFrame.Visible = false;
		GameManager.Singleton.Soundtrack.SetMuted(false);

		m_IntroBarrier.Destruct();
	}
}
