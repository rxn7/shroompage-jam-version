

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Game.ItemSystem;
using Game.Player;
using Godot;

namespace Game.Story;

internal static class StoryIntroPersistentData {
	public static bool DisableIntro = false;
}

// TODO this should really inherit from a base StoryElement class since it's referenced a lot
// TODO: THIS REALLY FUCKING NEEDS TO INHERIT A STORY BASE CLASS!!!!!!!!!!!!!!!!!!!! ^^^^
internal partial class StoryIntro : Node {
	public const int MushroomAmountToCollect = 6;

	[Export] private DirectionalLight3D m_DayLight, m_NightLight;
	[Export] private Godot.Environment m_DayEnv, m_NightEnv;
	[Export] private WorldEnvironment m_WorldEnv;

	public int CollectedShroomCount { get; set; } = 0;
	public event Action OnFinish;

	private Barrier m_IntroBarrier;
	private PlayerNotificationDisplay m_NotificationDisplay;
	private GameSoundtrack m_Soundtrack;    
	private PlayerManager m_Player;
	private Label m_ShroomCollectProgress;
	private AudioStreamPlayer m_EndSequenceSoundPlayer;

	private bool m_FinishedTextIntro = false;
	private bool m_FinishedIntro = false;

	private int m_CurrentMessage = 0;
	private float m_MessageTimer = 3.0f;
	public bool m_LastShroom { get; set; } = false;
	private readonly String[] m_Messages = {
		"Press [WASD] to move",
		"Press [Space] to jump",
		"Press [Left Shift] to sprint",
		"Press [F] to pick up",
		"Pick up 6 mushrooms"
	};

	public void Start(GameManager game) {
		m_Player = game.Player;
		m_NotificationDisplay = game.Player.NotificationDisplay;
		m_Soundtrack = game.Soundtrack;
   		m_ShroomCollectProgress = game.Player.HUD.GetNode<Label>("ShroomProgress");
		m_IntroBarrier = GetNode<Barrier>("IntroGate");
		m_EndSequenceSoundPlayer = GetNode<AudioStreamPlayer>("EndSequenceSoundPlayer");

		m_Player.ViewmodelDisabled = true;
		m_Soundtrack.SetIntroMusic(true);
		m_Soundtrack.SetMuted(false);

		GameManager.Singleton.Player.Headlight.Visible = false;

		MagicMushroom.DisableEffects = true;

		if (StoryIntroPersistentData.DisableIntro) {
			m_Player.GlobalPosition = m_IntroBarrier.GlobalPosition;
			FinishEndSequence();
			return;
		}

		MagicMushroom.Consumed += OnMagicMushroomConsumed;
	}

	public override void _ExitTree() {
		MagicMushroom.Consumed -= OnMagicMushroomConsumed;
	}

	public override void _Process(double delta_time) {
		if (m_FinishedIntro) {
			return;
		} 

		m_ShroomCollectProgress.Text = $"Mushrooms: {CollectedShroomCount}/6";
		TextIntroUpdate(delta_time);
	}

	private async void OnMagicMushroomConsumed(MagicMushroom shroom) {
		CollectedShroomCount++;
		
		if (CollectedShroomCount == MushroomAmountToCollect) {
			await BeginEndSequence();
			return;
		}

		if (CollectedShroomCount == MushroomAmountToCollect - 1)  {
			m_NotificationDisplay.DisplayNotification("I should eat one", 4);
			m_LastShroom = true;
		}
	}

	private void TextIntroUpdate(double timeDelta) {
		m_MessageTimer -= (float)timeDelta;

		if (m_LastShroom) return;
		if (m_FinishedTextIntro) return;
		if (m_MessageTimer > 0) return;

		m_MessageTimer = 3;
		m_NotificationDisplay.DisplayNotification(m_Messages[m_CurrentMessage], 3);
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

	private async Task BeginEndSequence() {
		m_EndSequenceSoundPlayer.Play();

		GameManager.Singleton.Player.BlackoutFrame.Visible = true;
		GameManager.Singleton.Player.Controller.Locked = true;
		GameManager.Singleton.Soundtrack.SetMuted(true);

		await Task.Delay(5000);

		FinishEndSequence();
	}

	private void FinishEndSequence() {
		GameManager.Singleton.Player.Controller.Locked = false;

		m_DayLight.Visible = false;
		m_WorldEnv.Environment = m_NightEnv;
		m_NightLight.Visible = true;

		GameManager.Singleton.Player.BlackoutFrame.Visible = false;

		GameManager.Singleton.Soundtrack.SetIntroMusic(false);
		GameManager.Singleton.Soundtrack.SetMuted(false);

		GameManager.Singleton.Player.ViewmodelDisabled = false;
		m_IntroBarrier.Destruct();

		m_NotificationDisplay.DisplayNotification("Press [LMB] to melee attack\nPress [E] to kick\nThe mushrooms are coming for you.", 6);
		SpawnMachete();

		m_ShroomCollectProgress.Text = "";
		m_FinishedIntro = true;
		MagicMushroom.DisableEffects = false;

		GameManager.Singleton.Player.Headlight.Visible = true;

		OnFinish?.Invoke();
		QueueFree();
	}

	private void SpawnMachete() {
		GameManager.Singleton.Player.ItemManager.HeldItem = PlayerItemManager.MacheteItemData.Spawn() as HoldableItem;
		AddChild(GameManager.Singleton.Player.ItemManager.HeldItem);
		GameManager.Singleton.Player.ItemManager.HeldItem.Equip(GameManager.Singleton.Player);
	}
}
