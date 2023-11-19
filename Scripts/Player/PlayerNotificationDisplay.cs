using Godot;
using Godot.Collections;
using Game.ItemSystem;
using System;

namespace Game.Player; 

internal partial class PlayerNotificationDisplay : Label {
	
    double m_TimeToClear = 0.0f;
    bool m_TextCleared = false;

    public override void _Process(double delta_time) {
        if (m_TextCleared) return;

        m_TimeToClear -= (float)delta_time;
        if (m_TimeToClear <= 0.0f) {
            Text = "";
            m_TextCleared = true;
        }
	}

    public void DisplayNotification(string text, float duration) {
        Text = text;
        m_TextCleared = false;
        m_TimeToClear = duration;
    }
}
