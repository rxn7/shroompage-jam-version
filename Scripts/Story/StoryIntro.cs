

using System;
using System.Threading.Tasks;
using Game.Player;
using Godot;

namespace Game.Story;

// TODO this should really inherit from a base StoryElement class since it's referenced a lot
internal partial class StoryIntro : Node {

    public bool DisableShroomEffects = true;

	private PlayerNotificationDisplay m_NotificationDisplay;
	private GameSoundtrack m_Soundtrack;    
	private PlayerManager m_Player;
	private Label m_ShroomCollectProgress;

    private int m_CurrentMessage = 0;
    private double m_MessageTimer = 3;
    private bool m_FinishedTextIntro = false;
    private int m_collectedShrooms = 0;
    private bool m_playedLastShroomNotification = false;
    private readonly String[] m_Messages = {
        "Press [WASD] to move",
        "Press [Space] to jump",
        "Press [Left Shift] to sprint",
        "Harvest 6 mushrooms",
    };

	private int collectedShrooms = 0;

	public async Task Start(GameManager game) {
		m_Player = game.Player;
		m_NotificationDisplay = game.Player.NotificationDisplay;
		m_Soundtrack = game.Soundtrack;
   		m_ShroomCollectProgress = game.Player.HUD.GetNode<Label>("ShroomProgress");

		m_Player.ViewmodelDisabled = true;
		m_Soundtrack.SetIntroMusic(true);

		// it makes a loud sound :(
		await Task.Delay(600).ContinueWith(t => {
			m_Soundtrack.SetMuted(false);
		});
	}

	public override void _Process(double delta_time) {
        m_ShroomCollectProgress.Text = $"Mushrooms: {m_collectedShrooms}/6";
        TextIntroUpdate(delta_time);
		m_ShroomCollectProgress.Text = $"Mushrooms: {collectedShrooms}/6";
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

    public void CollectShroom() {
        m_collectedShrooms++;
        
        if (m_collectedShrooms == 6) {
            // ending sequence

            return;
        }

        if (m_collectedShrooms != 5) return;
        m_playedLastShroomNotification = true;
        m_NotificationDisplay.DisplayNotification("I should eat one of these", 3);
    }

}
