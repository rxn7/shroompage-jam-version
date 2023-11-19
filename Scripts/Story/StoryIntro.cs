

using Game.Player;
using Godot;

namespace Game.Story;

internal partial class StoryIntro : Node {

    PlayerNotificationDisplay m_NotificationDisplay;
    PlayerManager m_Player;

	public void Start(PlayerManager player) {
        m_Player = player;
        m_NotificationDisplay = player.NotificationDisplay;

        m_Player.ViewmodelDisabled = true;
        m_NotificationDisplay.DisplayNotification("Press [WASD] to move", 5);
        GD.Print("StoryIntro: Start");
	}

	public override void _Process(double delta_time) {

	}

}
