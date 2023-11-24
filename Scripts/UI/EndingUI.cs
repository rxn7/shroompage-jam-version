using System;
using System.Text;
using Godot;

namespace Game.UI;

internal partial class EndingUI : Control {
    private Label m_Label;

    public override void _Ready() {
        TimeSpan speedrunTime = TimeSpan.FromMilliseconds(SpeedrunData.SpeedrunEndMsec - SpeedrunData.SpeedrunStartMsec);

        StringBuilder speedrunTimeText = new();
        if(Mathf.FloorToInt(speedrunTime.Minutes) > 0)
            speedrunTimeText.Append($"{speedrunTime.Minutes:0} minutes and ");

        speedrunTimeText.Append($"{speedrunTime.Seconds:00}.{speedrunTime.Milliseconds:000} seconds");

        m_Label = GetNode<Label>("EndingLabel");
        m_Label.Text = $"Congratulations! You commited arson.\nAlso, you stopped the world ending, I guess.\nYou wasted of {speedrunTimeText} your life.";
    }
}